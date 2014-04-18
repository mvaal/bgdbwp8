using BoardGames.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BoardGames.ViewModels
{
    public class TopListViewModel : INotifyPropertyChanged
    {
        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private IndexedObservableCollection<IndexedBoardGame> boardGames;
        public IndexedObservableCollection<IndexedBoardGame> BoardGames
        {
            get
            {
                return boardGames;
            }
            private set
            {
                if (boardGames != value)
                {
                    boardGames = value;
                    NotifyPropertyChanged("BoardGames");
                }
            }
        }
        public ObservableCollection<int> BoardGameIds { get; private set; }

        private bool isLoading = false;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    NotifyPropertyChanged("IsLoading");
                    NotifyPropertyChanged("Visibility");
                }
            }
        }

        public Visibility Visibility
        {
            get
            {
                return isLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private BggTopListParser parser;
        private BoardGameCache cache;
        private int loadCount = 20;

        public TopListViewModel()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            BoardGames = new IndexedObservableCollection<IndexedBoardGame>();
            cache = BoardGameCache.Instance;
        }

        internal async Task Init(string gameType)
        {
            if (BoardGameIds == null)
            {
                parser = new BggTopListParser(gameType);
                IsLoading = true;
                try
                {
                    BoardGameIds = new ObservableCollection<int>(await parser.NextPage());
                }
                finally
                {
                    IsLoading = false;
                }
                await LoadPage();
            }
        }

        internal async Task LoadPage()
        {
            int takeCount = BoardGameIds.Count() - BoardGames.Count();
            if (takeCount > 0)
            {
                IsLoading = true;
                try
                {
                    if (takeCount > loadCount)
                    {
                        takeCount = loadCount;
                    }
                    var subSet = BoardGameIds.Skip(BoardGames.Count()).Take(takeCount);
                    var count = subSet.Count();
                    foreach (var boardGame in await cache.BoardGamesFromIds(subSet))
                    {
                        BoardGames.Add(new IndexedBoardGame(boardGame));
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        internal void Unload()
        {
            BoardGames = new IndexedObservableCollection<IndexedBoardGame>();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class IndexedBoardGame : IIndex
    {
        public int Index { get; set; }
        public BoardGame BoardGame { get; set; }

        public IndexedBoardGame()
        {

        }

        public IndexedBoardGame(BoardGame boardGame)
        {
            BoardGame = boardGame;
        }
    }
}
