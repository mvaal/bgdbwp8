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

namespace BoardGames
{
    public partial class AdControl : UserControl
    {
        private string pubCenterAdUnitId;
        public string PubCenterAdUnitId
        {
            get
            {
                return pubCenterAdUnitId;
            }
            set
            {
                pubCenterAdUnitId = value;
                AdUnit.AdUnitId = value;
            }
        }
        private string pubCenterApplicationId;
        public string PubCenterApplicationId
        {
            get
            {
                return pubCenterApplicationId;
            }
            set
            {
                pubCenterApplicationId = value;
                AdUnit.ApplicationId = value;
            }
        }
        private string adDuplexAppId;
        public string AdDuplexAppId
        {
            get
            {
                return adDuplexAppId;
            }
            set
            {
                pubCenterApplicationId = adDuplexAppId;
                AdDuplexAd.AppId = value;
            }
        }

        public AdControl()
        {
            InitializeComponent();
        }

        private void AdUnit_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            AdUnit.Visibility = Visibility.Collapsed;
            AdDuplexAd.Visibility = Visibility.Visible;
            Debug.WriteLine("Error");
        }

        private void AdDuplexAd_AdLoadingError(object sender, AdDuplex.AdLoadingErrorEventArgs e)
        {
            Debug.WriteLine("Error");
        }
    }
}
