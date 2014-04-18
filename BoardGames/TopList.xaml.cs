using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BoardGames.ViewModels;
using System.Windows.Input;

namespace BoardGames
{
    public partial class TopList : PhoneApplicationPage
    {
        private TopListViewModel model;
        private int offsetKnob = 10;

        public TopList()
        {
            InitializeComponent();
            model = new TopListViewModel();
            DataContext = model;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            model.Title = NavigationContext.QueryString["title"];

            var gameType = NavigationContext.QueryString["gametype"];
            try
            {
                await model.Init(gameType);
            }
            catch (Exception we)
            {
                Console.WriteLine(we);
                //var errorBox = Utility.DisplayExceptionCustomMessageBox(we);
                //errorBox.Show();
            }
        }

        private void BoardGameHeader_SearchClicked(object sender, RoutedEventArgs e)
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

        private async void TopListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            var list = (LongListSelector)sender;
            var group = (TopListViewModel)list.DataContext;

            if (!group.IsLoading && list.ItemsSource != null && list.ItemsSource.Count >= offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as IndexedBoardGame).Equals(list.ItemsSource[list.ItemsSource.Count - offsetKnob]))
                    {
                        Console.WriteLine(String.Format("Searching for {0}", offsetKnob));
                        try
                        {
                            await group.LoadPage();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            //var errorBox = Utility.DisplayExceptionCustomMessageBox(ex);
                            //errorBox.Show();
                        }
                    }
                }
            }
        }

        private void TopListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var source = (LongListSelector)sender;
            IndexedBoardGame item = (IndexedBoardGame)source.SelectedItem;
            if (source.SelectedItem == null)
                return;

            NavigationService.Navigate(new Uri(String.Format("/DetailsPage.xaml?objectId={0}", item.BoardGame.ObjectId), UriKind.Relative));

            source.SelectedItem = null;
        }
    }
}