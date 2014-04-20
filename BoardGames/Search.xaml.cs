using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Threading.Tasks;
using BoardGames.ViewModels;
using System.Windows.Input;
using Coding4Fun.Toolkit.Controls;
using BugSense;
using BugSense.Core.Model;

namespace BoardGames
{
    public partial class Search : PhoneApplicationPage
    {
        private SearchViewModel model;
        private int offsetKnob = 10;

        public Search()
        {
            InitializeComponent();
            model = new SearchViewModel();
            this.DataContext = model;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var searchText = NavigationContext.QueryString["searchText"];
            if (searchText != null)
            {
                SearchOverlay.SearchBox.Text = searchText;
                await DoSearch();
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchOverlay.Visibility = Visibility.Collapsed;
        }

        private async void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SearchOverlay.SearchBox.Text.Length > 0)
            {
                this.Focus();
                await DoSearch();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchOverlay.Visibility = Visibility.Visible;
            SearchOverlay.SearchBox.Focus();
        }

        private async Task DoSearch()
        {
            // Disable Search Input
            SearchOverlay.SearchBox.IsEnabled = false;
            string gameName = SearchOverlay.SearchBox.Text;
            SearchResultTextBlock.Text = gameName;
            Exception exception = null;
            try
            {
                var model = DataContext as SearchViewModel;
                await model.Search(gameName);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                SearchOverlay.SearchBox.IsEnabled = true;
            }
            if (exception != null)
            {
                await ShowToastException("Error while searching", gameName, exception);
            }
        }

        private async Task ShowToastException(String title, string gameName, Exception exception)
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
            LimitedCrashExtraDataList extrasExtraDataList = new LimitedCrashExtraDataList
            {
                new CrashExtraData("Search", gameName)
            };
            BugSenseLogResult result = await BugSenseHandler.Instance.LogExceptionAsync(exception, extrasExtraDataList);
            Debug.WriteLine("Client Request: {0}", result.ClientRequest);
        }

        private void BoardGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBoardGame = (BoardGame)BoardGames.SelectedItem;
            if (BoardGames.SelectedItem == null)
                return;

            Debug.WriteLine(String.Format("Selected game {0}, id {1}", selectedBoardGame.Name, selectedBoardGame.ObjectId));

            NavigationService.Navigate(new Uri("/DetailsPage.xaml?objectId=" + selectedBoardGame.ObjectId, UriKind.Relative));

            BoardGames.SelectedItem = null;
        }

        private async void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await DoSearch();
            }
        }

        private async void SearchTextBox_ActionIconTapped(object sender, EventArgs e)
        {
            await DoSearch();
        }

        private async void BoardGames_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!model.IsLoading && BoardGames.ItemsSource != null && BoardGames.ItemsSource.Count >= offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as BoardGame).Equals(BoardGames.ItemsSource[BoardGames.ItemsSource.Count - offsetKnob]))
                    {
                        Debug.WriteLine(String.Format("Searching for {0}", offsetKnob));
                        try
                        {
                            await model.LoadPage();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            //var exceptionBox = Utility.DisplayExceptionCustomMessageBox(ex);
                            //exceptionBox.Show();
                        }
                    }
                }
            }
        }

    }
}