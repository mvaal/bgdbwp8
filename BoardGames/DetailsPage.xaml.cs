using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BoardGames.Resources;
using BoardGames.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;

namespace BoardGames
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private readonly DetailsViewModel model;

        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
            TiltEffect.TiltableItems.Add(typeof(HubTile));
            model = new DetailsViewModel();
            model.PropertyChanged += model_PropertyChanged;

            List<ExploreItem> exploreList = new List<ExploreItem>();
            //Add your playlistitems in PlayList
            exploreList.Add(new ExploreItem() { ItemName = "share this link", ExploreGroup = "actions", Id = "share" });
            exploreList.Add(new ExploreItem() { ItemName = "email this link", ExploreGroup = "actions", Id = "email" });
            exploreList.Add(new ExploreItem() { ItemName = "view on bgg.com", ExploreGroup = "actions", Id = "web" });

            var groupedExploreList =
                    from list in exploreList
                    group list by list.ExploreGroup into listByGroup
                    select new KeyedList<string, ExploreItem>(listByGroup);
            List<KeyedList<string, ExploreItem>> exploreKeyedList = new List<KeyedList<string, ExploreItem>>(groupedExploreList);
            model.ExploreKeyedList = new ObservableCollection<KeyedList<string, ExploreItem>>(exploreKeyedList);

            this.DataContext = model;
        }

        private async void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ObjectId":
                    try
                    {
                        await model.LoadBoardGameAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        //var exceptionBox = Utility.DisplayExceptionCustomMessageBox(ex);
                        //exceptionBox.Dismissed += (s1, e1) =>
                        //{
                        //    // This is a hack to prevent null pointer exception (bug in wptoolkit)
                        //    // Happens when navigating from within CustomMesageBox
                        //    ((CustomMessageBox)s1).Dismissing += (o, e3) => e3.Cancel = true;
                        //    NavigationService.GoBack();
                        //};
                        //exceptionBox.Show();
                    }
                    finally
                    {
                        //ToggleOverlay(false);
                    }
                    break;
                case "BoardGame":
                    //DescriptionWebBrowser.NavigateToString(model.BoardGame.Description);
                    try
                    {
                        RankListSelector.Visibility = model.BoardGame.RankList.Any() ? Visibility.Visible : Visibility.Collapsed;
                        await model.LoadExpansionsAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var boardGameId = NavigationContext.QueryString["objectId"];
            model.ObjectId = boardGameId;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchOverlay.Visibility = Visibility.Visible;
            SearchOverlay.SearchBox.Focus();
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchOverlay.Visibility = Visibility.Collapsed;
        }

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SearchOverlay.SearchBox.Text.Length > 0)
            {
                NavigationService.Navigate(new Uri("/Search.xaml?searchText=" + SearchOverlay.SearchBox.Text, UriKind.Relative));
            }
        }

        private void ExploreList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExploreList.SelectedItem == null)
                return;

            var exploreItem = ExploreList.SelectedItem as ExploreItem;
            switch (exploreItem.Id)
            {
                case "share":
                    ShareLinkTask shareLinkTask = new ShareLinkTask();

                    shareLinkTask.Title = String.Format("Board Games - {0}", model.BoardGame.Name);
                    shareLinkTask.LinkUri = new Uri(String.Format("http://boardgamegeek.com/boardgame/{0}", model.BoardGame.ObjectId), UriKind.Absolute);
                    shareLinkTask.Message = String.Format("Check out this board game!");

                    shareLinkTask.Show();
                    break;
                case "email":
                    EmailComposeTask emailComposeTask = new EmailComposeTask();

                    emailComposeTask.Subject = String.Format("Board Games - {0}", model.BoardGame.Name);
                    emailComposeTask.Body = String.Format("Check out this board game!\nhttp://boardgamegeek.com/boardgame/{0}", model.BoardGame.ObjectId);

                    emailComposeTask.Show();
                    break;
                case "web":
                    WebBrowserTask webBrowserTask = new WebBrowserTask();

                    webBrowserTask.Uri = new Uri(String.Format("http://boardgamegeek.com/boardgame/{0}", model.BoardGame.ObjectId), UriKind.Absolute);

                    webBrowserTask.Show();
                    break;
            }

            ExploreList.SelectedItem = null;
        }

        private void ExpansionListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var source = (LongListSelector)sender;
            BoardGame boardGame = (BoardGame)source.SelectedItem;
            if (source.SelectedItem == null)
                return;

            NavigationService.Navigate(new Uri(String.Format("/DetailsPage.xaml?objectId={0}", boardGame.ObjectId), UriKind.Relative));

            source.SelectedItem = null;
        }

        private void RankListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var source = (LongListSelector)sender;
            Rank rank = (Rank)source.SelectedItem;
            if (source.SelectedItem == null)
                return;

            string uri = null;
            string type = rank.FriendlyName;
            if(type.Equals("Board Game Rank")) {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.All, AppResources.TopListsAll);
            }
            else if (type.Equals("Abstract Games Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Abstract, AppResources.TopListsAbstract);
            }
            else if (type.Equals("Children's Games Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Childrens, AppResources.TopListsChildrens);
            }
            else if (type.Equals("Customizable Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Customizable, AppResources.TopListsCustomizable);
            }
            else if (type.Equals("Family Game Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Family, AppResources.TopListsFamily);
            }
            else if (type.Equals("Party Game Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Party, AppResources.TopListsParty);
            }
            else if (type.Equals("Strategy Game Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Strategy, AppResources.TopListsStrategy);
            }
            else if (type.Equals("Thematic Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.Thematic, AppResources.TopListsThematic);
            }
            else if (type.Equals("War Game Rank"))
            {
                uri = String.Format("/TopList.xaml?gametype={0}&title={1}", BoardGameType.War, AppResources.TopListsWar);
            }

            if (uri != null)
            {
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else
            {
                Console.WriteLine(String.Format("Unknown type: {0}", rank.FriendlyName));
            }

            source.SelectedItem = null;
        }
    }
}