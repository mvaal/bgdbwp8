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
using Coding4Fun.Toolkit.Controls;
using BugSense.Core.Model;
using BugSense;
using System.Threading.Tasks;

namespace BoardGames
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private readonly DetailsViewModel model;

        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
            Microsoft.Phone.Controls.TiltEffect.TiltableItems.Add(typeof(HubTile));
            model = new DetailsViewModel();
            model.PropertyChanged += model_PropertyChanged;

            this.DataContext = model;
        }

        private async void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Exception exception = null;
            switch (e.PropertyName)
            {
                case "ObjectId":
                    try
                    {
                        await model.LoadBoardGameAsync();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        //ToggleOverlay(false);
                    }
                    if (exception != null)
                    {
                        await ShowToastException("Error loading details", exception);
                    }
                    break;
                case "BoardGame":
                    try
                    {
                        RankListSelector.Visibility = model.BoardGame.RankList.Any() ? Visibility.Visible : Visibility.Collapsed;
                        await model.LoadExpansionsAsync();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    if (exception != null)
                    {
                        await ShowToastException("Error loading expansions", exception);
                    }
                    break;
            }
        }

        private async Task ShowToastException(String title, Exception exception)
        {
            ToastPrompt toast = new ToastPrompt()
            {
                Title = title,
                Message = exception.Message,
                TextOrientation = System.Windows.Controls.Orientation.Vertical,
                MillisecondsUntilHidden = 4000
            };
            //toast.ImageSource = new BitmapImage(new Uri("ApplicationIcon.png", UriKind.RelativeOrAbsolute));
            //toast.Completed += toast_Completed;
            toast.Show();
            BugSenseLogResult result = await BugSenseHandler.Instance.LogExceptionAsync(exception);
            Debug.WriteLine("Client Request: {0}", result.ClientRequest);
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
            if (exploreItem.Id == DetailsViewModel.SHARE_ID)
            {
                ShareLinkTask shareLinkTask = new ShareLinkTask();

                shareLinkTask.Title = String.Format("{0} - {1}", AppResources.ApplicationTitle, model.BoardGame.Name);
                shareLinkTask.LinkUri = new Uri(String.Format("http://boardgamegeek.com/boardgame/{0}", model.BoardGame.ObjectId), UriKind.Absolute);
                shareLinkTask.Message = AppResources.DetailsExploreActionsShareMessage;

                shareLinkTask.Show();
            }
            else if (exploreItem.Id == DetailsViewModel.EMAIL_ID)
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();

                emailComposeTask.Subject = String.Format("{0} - {1}", AppResources.ApplicationTitle, model.BoardGame.Name);
                emailComposeTask.Body = String.Format("{0}\nhttp://boardgamegeek.com/boardgame/{1}", AppResources.DetailsExploreActionsEmailMessage, model.BoardGame.ObjectId);

                emailComposeTask.Show();
            }
            else if (exploreItem.Id == DetailsViewModel.WEB_ID)
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();

                webBrowserTask.Uri = new Uri(String.Format("http://boardgamegeek.com/boardgame/{0}", model.BoardGame.ObjectId), UriKind.Absolute);

                webBrowserTask.Show();
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

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(String.Format("/DescriptionPage.xaml?descriptionTitle={0}&objectId={1}", "description", model.ObjectId), UriKind.Relative));
        }
    }
}