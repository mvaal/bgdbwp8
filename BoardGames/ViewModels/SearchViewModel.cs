using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BoardGame> searchResult;
        public ObservableCollection<BoardGame> SearchResult
        {
            get
            {
                return searchResult;
            }
            set
            {
                if (searchResult != value)
                {
                    searchResult = value;
                    NotifyPropertyChanged("SearchResult");
                }
            }
        }

        private ObservableCollection<int> searchResultIds;
        public ObservableCollection<int> SearchResultIds
        {
            get
            {
                return searchResultIds;
            }
            set
            {
                if (searchResultIds != value)
                {
                    searchResultIds = value;
                    NotifyPropertyChanged("SearchResultIds");
                }
            }
        }

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
                }
            }
        }

        private BoardGameCache cache;
        private int loadCount = 20;

        public SearchViewModel(int loadCount)
        {
            InitializeData();
            this.loadCount = loadCount;
        }

        public SearchViewModel()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            SearchResult = new ObservableCollection<BoardGame>();
            cache = BoardGameCache.Instance;
        }

        public async Task Search(string gameName)
        {
            if (!String.IsNullOrWhiteSpace(gameName))
            {
                SearchResult = new ObservableCollection<BoardGame>();

                IsLoading = true;
                try
                {
                    SearchResultIds = new ObservableCollection<int>(await cache.BoardGameIds(gameName));
                }
                finally
                {
                    IsLoading = false;
                }
                await LoadPage();
                //SearchResult = new ObservableCollection<BoardGame>(await cache.BoardGamesFromName(gameName));
            }
        }

        public async Task LoadPage()
        {
            int takeCount = SearchResultIds.Count() - SearchResult.Count();
            if (takeCount > 0)
            {
                IsLoading = true;
                try
                {
                    if (takeCount > loadCount)
                    {
                        takeCount = loadCount;
                    }
                    var subSet = SearchResultIds.Skip(SearchResult.Count()).Take(takeCount);
                    var count = subSet.Count();
                    foreach (var boardGame in await cache.BoardGamesFromIds(subSet))
                    {
                        SearchResult.Add(boardGame);
                    }
                }
                finally
                {
                    IsLoading = false;
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
}
