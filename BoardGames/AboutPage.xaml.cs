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
using Microsoft.Phone.Tasks;

namespace BoardGames
{
    public partial class AboutPage : PhoneApplicationPage
    {
        private AboutViewModel model;

        public AboutPage()
        {
            InitializeComponent();
            model = new AboutViewModel();
            DataContext = model;
        }

        private void RateHubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void FeedbackHubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FeedbackPage.xaml", UriKind.Relative));
        }

        private void AppsHubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();
            marketplaceSearchTask.SearchTerms = "2Pawns";
            marketplaceSearchTask.Show();
        }
    }
}