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
using BugSense;
using BugSense.Core.Model;

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
                adDuplexAppId = value;
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
            if (AdDuplexAppId != null)
            {
                AdDuplexAd.Visibility = Visibility.Visible;
            }
            // Don't want to waste error logging in bug sense
            //BugSenseLogResult result = await BugSenseHandler.Instance.LogExceptionAsync(e.Error);
            //Debug.WriteLine("Client Request: {0}", result.ClientRequest);
            Debug.WriteLine("PubCenter Ad Unit Failure: {0}", e.Error);
        }

        private void AdDuplexAd_AdLoadingError(object sender, AdDuplex.AdLoadingErrorEventArgs e)
        {
            // Don't want to waste error logging in bug sense
            //BugSenseLogResult result = await BugSenseHandler.Instance.LogExceptionAsync(e.Error);
            //Debug.WriteLine("Client Request: {0}", result.ClientRequest);
            Debug.WriteLine("AdDuplex Ad Unit Failure: {0}", e.Error);
        }
    }
}
