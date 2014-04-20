using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using BoardGames.ViewModels;
using Coding4Fun.Toolkit.Controls;
using System.Threading.Tasks;
using BugSense;
using BugSense.Core.Model;
using System.Diagnostics;
using System.ComponentModel;

namespace BoardGames
{
    public partial class DescriptionPage : PhoneApplicationPage
    {
        private readonly DescriptionViewModel model;

        public DescriptionPage()
        {
            InitializeComponent();
            model = new DescriptionViewModel();
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
                    if (exception != null)
                    {
                        await ShowToastException("Error loading details", exception);
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

            var descriptionTitle = NavigationContext.QueryString["descriptionTitle"];
            model.DescriptionTitle = descriptionTitle;

            var boardGameId = NavigationContext.QueryString["objectId"];
            model.ObjectId = boardGameId;
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchOverlay.Visibility = Visibility.Visible;
            SearchOverlay.SearchBox.Focus();
        }
    }
}