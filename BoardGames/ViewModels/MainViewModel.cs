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
                new TopListsMenuItem("all", AppResources.TopListsAll,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.All, AppResources.TopListsAll),"/Assets/Images/all.png"),
                new TopListsMenuItem("abstract", AppResources.TopListsAbstract,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Abstract,AppResources.TopListsAbstract),"/Assets/Images/abstract.png"),
                new TopListsMenuItem("childrens",AppResources.TopListsChildrens, String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Childrens,AppResources.TopListsChildrens),"/Assets/Images/children.png"),
                new TopListsMenuItem("customizable", AppResources.TopListsCustomizable,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Customizable,AppResources.TopListsCustomizable),"/Assets/Images/custom.png"),
                new TopListsMenuItem("family",AppResources.TopListsFamily, String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Family,AppResources.TopListsFamily),"/Assets/Images/family.png"),
                new TopListsMenuItem("party", AppResources.TopListsParty,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Party,AppResources.TopListsParty),"/Assets/Images/party.png"),
                new TopListsMenuItem("strategy", AppResources.TopListsStrategy,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Strategy,AppResources.TopListsStrategy),"/Assets/Images/strategy.png"),
                new TopListsMenuItem("thematic", AppResources.TopListsThematic,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Thematic,AppResources.TopListsThematic),"/Assets/Images/thematic.png"),
                new TopListsMenuItem("war", AppResources.TopListsWar,String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.War,AppResources.TopListsWar),"/Assets/Images/war.png"),
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
        public string UniqueId { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public string Image { get; set; }

        public TopListsMenuItem()
        {

        }

        public TopListsMenuItem(string uniqueId, string header, string url, string image)
        {
            UniqueId = uniqueId;
            Url = url;
            Header = header;
            Image = image;
        }
    }

}