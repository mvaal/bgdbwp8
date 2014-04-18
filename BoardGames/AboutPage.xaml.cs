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

        private void AboutLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var source = (LongListSelector)sender;
            AboutMenuItem aboutMenuItem = (AboutMenuItem)source.SelectedItem;
            if (source.SelectedItem == null)
                return;

            switch (aboutMenuItem.UniqueId)
            {
                case "rate":
                    MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                    marketplaceReviewTask.Show();
                    break;
                case "feedback":
                    NavigationService.Navigate(new Uri("/FeedbackPage.xaml", UriKind.Relative));
                    break;
                case "apps":
                    MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();
                    marketplaceSearchTask.SearchTerms = "2Pawns";
                    marketplaceSearchTask.Show();
                    break;
            }

            source.SelectedItem = null;
        }
    }
}