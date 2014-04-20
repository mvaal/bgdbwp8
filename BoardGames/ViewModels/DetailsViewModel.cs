using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BoardGames.ViewModels
{
    public class DetailsViewModel : INotifyPropertyChanged
    {
        private BoardGameCache cache;

        private string objectId;
        public string ObjectId
        {
            get
            {
                return objectId;
            }
            set
            {
                if (objectId != value)
                {
                    objectId = value;
                    NotifyPropertyChanged("ObjectId");
                }
            }
        }

        private BoardGame boardGame;
        public BoardGame BoardGame
        { 
            get
            {
                return boardGame;
            }
            set
            {
                if (boardGame != value)
                {
                    boardGame = value;
                    NotifyPropertyChanged("BoardGame");
                }
            }
        }

        private ObservableCollection<KeyedList<string, ExploreItem>> exploreKeyedList;
        public ObservableCollection<KeyedList<string, ExploreItem>> ExploreKeyedList
        {
            get
            {
                return exploreKeyedList;
            }
            set
            {
                if (exploreKeyedList != value)
                {
                    exploreKeyedList = value;
                    NotifyPropertyChanged("ExploreKeyedList");
                }
            }
        }

        private ObservableCollection<BoardGame> expansions;
        public ObservableCollection<BoardGame> Expansions
        {
            get
            {
                return expansions;
            }
            set
            {
                if (expansions != value)
                {
                    expansions = value;
                    NotifyPropertyChanged("Expansions");
                }
            }
        }

        private Visibility expansionsVisible;
        public Visibility ExpansionsVisible
        {
            get
            {
                return expansionsVisible;
            }
            set
            {
                if (expansionsVisible != value)
                {
                    expansionsVisible = value;
                    NotifyPropertyChanged("ExpansionsVisible");
                }
            }
        }

        public DetailsViewModel()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            cache = BoardGameCache.Instance;
            // Fake board game with temp title
            boardGame = new BoardGame()
            {
                Name = " "
            };
            expansions = new ObservableCollection<BoardGame>(Enumerable.Empty<BoardGame>());
            expansionsVisible = Visibility.Visible;
        }

        public async Task LoadBoardGameAsync()
        {
            if (objectId == null)
            {
                Debug.WriteLine("Attemted to load Board Game with no object id");
                return;
            }
            int id;
            if (int.TryParse(objectId, out id))
            {
                BoardGame = (await cache.BoardGamesFromIds(id)).First();
            }
            else
            {
                Debug.WriteLine(String.Format("Unable to parse id {0} ", objectId));
            }
        }

        public async Task LoadExpansionsAsync()
        {
            if (boardGame == null)
            {
                Debug.WriteLine("Attemted to load Board Game Expansions with no Board Game");
                return;
            }
            var bgExpansions = boardGame.BoardGameExpansions;
            if (bgExpansions == null || !bgExpansions.Any())
            {
                ExpansionsVisible = Visibility.Collapsed;
                return;
            }
            var expIds = bgExpansions.Where(bg => !bg.Inbound).Select(bg => bg.ObjectId);
            try
            {
                Expansions = new ObservableCollection<BoardGame>(await cache.BoardGamesFromIds(expIds));
            }
            finally
            {
                if (Expansions == null || !Expansions.Any())
                {
                    ExpansionsVisible = Visibility.Collapsed;
                }
                else
                {
                    ExpansionsVisible = Visibility.Visible;
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class ExploreItem
    {
        public string ItemName { get; set; }
        public string ExploreGroup { get; set; }
        public string Id { get; set; }
    }

    public class KeyedList<TKey, TItem> : List<TItem>
    {
        public TKey Key { protected set; get; }

        public KeyedList(TKey key, IEnumerable<TItem> items)
            : base(items)
        {
            Key = key;
        }

        public KeyedList(IGrouping<TKey, TItem> grouping)
            : base(grouping)
        {
            Key = grouping.Key;
        }
    }
}
