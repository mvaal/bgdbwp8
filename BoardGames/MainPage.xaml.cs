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
using System.Windows.Media;
using System.Windows.Input;
using BugSense;
using System.Diagnostics;
using BugSense.Core.Model;

namespace BoardGames
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BugSenseHandler.Instance.UnhandledExceptionHandled += (sender, response) =>
                Debug.WriteLine("Exception of type {0} handled by BugSense\r\nClient Request: {1}",
                response.ExceptionObject.GetType(),
                response.ClientJsonRequest);

            BugSenseHandler.Instance.LoggedRequestHandled += (sender, args) =>
                {
                    if (args.BugSenseLoggedResponseResult.RequestType == BugSenseRequestType.Event)
                    {
                        Debug.WriteLine("Logged Request {0}\r\nServer Response: {1}",
                            args.BugSenseLoggedResponseResult.ClientRequest,
                            args.BugSenseLoggedResponseResult.ServerResponse);
                    }
                    else
                    {
                        Debug.WriteLine("Logged Request {0}\r\nServer Response: {1}\r\nErrorId: {2}\r\nResolved: {3}",
                            args.BugSenseLoggedResponseResult.ClientRequest,
                            args.BugSenseLoggedResponseResult.ServerResponse,
                            args.BugSenseLoggedResponseResult.ErrorId,
                            args.BugSenseLoggedResponseResult.IsResolved ? "Yes" : "No");
                    }
                };

            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BugSenseHandler.Instance.RegisterAsyncHandlerContext();

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainLongListSelector.SelectedItem == null)
                return;

            var menuItem = MainLongListSelector.SelectedItem as TopListsMenuItem;
            NavigationService.Navigate(new Uri(menuItem.Url, UriKind.Relative));

            MainLongListSelector.SelectedItem = null;
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

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}