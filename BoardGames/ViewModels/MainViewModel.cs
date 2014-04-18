using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BoardGames.Resources;

namespace BoardGames.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TopListsMenuItem> topListsMenuItems;
        public ObservableCollection<TopListsMenuItem> TopListsMenuItems
        {
            get
            {
                return topListsMenuItems;
            }
            private set
            {
                if (topListsMenuItems != value)
                {
                    topListsMenuItems = value;
                    NotifyPropertyChanged("TopListsMenuItems");
                }
            }
        }

        public MainViewModel()
        {
            //InitializeData();
        }

        public void LoadData()
        {
            TopListsMenuItem[] topListsMenuItems = new TopListsMenuItem[] {
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.All, AppResources.TopListsAll),AppResources.TopListsAll,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Abstract,AppResources.TopListsAbstract),AppResources.TopListsAbstract,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Childrens,AppResources.TopListsChildrens),AppResources.TopListsChildrens,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Customizable,AppResources.TopListsCustomizable),AppResources.TopListsCustomizable,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Family,AppResources.TopListsFamily),AppResources.TopListsFamily,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Party,AppResources.TopListsParty),AppResources.TopListsParty,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Strategy,AppResources.TopListsStrategy),AppResources.TopListsStrategy,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Thematic,AppResources.TopListsThematic),AppResources.TopListsThematic,null),
                new TopListsMenuItem(String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.War,AppResources.TopListsWar),AppResources.TopListsWar,null),
            };
            TopListsMenuItems = new ObservableCollection<TopListsMenuItem>(topListsMenuItems);

            this.IsDataLoaded = true;
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class TopListsMenuItem
    {
        public TopListsMenuItem()
        {

        }

        public TopListsMenuItem(string url, string header, string description)
        {
            Url = url;
            Header = header;
            Description = description;
        }

        public string Url { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
    }
}