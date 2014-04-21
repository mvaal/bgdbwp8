using BoardGames.Resources;
using BoardGames.Utilities;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.ViewModels
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<AboutMenuItem> aboutMenuItems;
        public ObservableCollection<AboutMenuItem> AboutMenuItems
        {
            get
            {
                return aboutMenuItems;
            }
            private set
            {
                if (aboutMenuItems != value)
                {
                    aboutMenuItems = value;
                    NotifyPropertyChanged("AboutMenuItems");
                }
            }
        }

        private ObservableCollection<HistoryMenuItem> historyMenuItems;
        public ObservableCollection<HistoryMenuItem> HistoryMenuItems
        {
            get
            {
                return historyMenuItems;
            }
            private set
            {
                if (historyMenuItems != value)
                {
                    historyMenuItems = value;
                    NotifyPropertyChanged("HistoryMenuItems");
                }
            }
        }

        private ObservableCollection<CreditsMenuItem> creditsMenuItems;
        public ObservableCollection<CreditsMenuItem> CreditsMenuItems
        {
            get
            {
                return creditsMenuItems;
            }
            private set
            {
                if (creditsMenuItems != value)
                {
                    creditsMenuItems = value;
                    NotifyPropertyChanged("CreditsMenuItems");
                }
            }
        }

        private string version;
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                if (version != value)
                {
                    version = value;
                    NotifyPropertyChanged("Version");
                }
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string author;
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                if (author != value)
                {
                    author = value;
                    NotifyPropertyChanged("Author");
                }
            }
        }

        private string publisher;
        public string Publisher
        {
            get
            {
                return publisher;
            }
            set
            {
                if (publisher != value)
                {
                    publisher = value;
                    NotifyPropertyChanged("Publisher");
                }
            }
        }

        public AboutViewModel()
        {
            Version = PhoneHelper.GetAppAttribute("Version");
            Title = PhoneHelper.GetAppAttribute("Title");
            Author = PhoneHelper.GetAppAttribute("Author");
            Publisher = PhoneHelper.GetAppAttribute("Publisher");

            AboutMenuItem[] aboutMenuItems = new AboutMenuItem[] {
                new AboutMenuItem("rate" , AppResources.AboutAboutRateHeader, "post a 5-star rating in the windows phone store", "/Assets/Images/heart.png"),
                new AboutMenuItem("feedback" , AppResources.AboutAboutFeedbackHeader, "tech support, feature requests, and suggestions", "/Assets/Images/feedback.png"),
                new AboutMenuItem("apps" , AppResources.AboutAboutAppsHeader, "more apps by this developer", "/Assets/Images/windows.png"),
            };
            AboutMenuItems = new ObservableCollection<AboutMenuItem>(aboutMenuItems);

            HistoryMenuItem[] historyMenuItem = new HistoryMenuItem[] {
                new HistoryMenuItem("1.0.0.0","Initial public release." ),
            };
            HistoryMenuItems = new ObservableCollection<HistoryMenuItem>(historyMenuItem.Reverse());

            CreditsMenuItem[] creditsMenuItems = new CreditsMenuItem[] {
                new CreditsMenuItem("http://silverlight.codeplex.com","Microsoft Silverlight Toolkit" ),
                new CreditsMenuItem("http://boardgamegeek.com","Board Game Geek" ),
                new CreditsMenuItem("http://htmlagilitypack.codeplex.com/releases/view/90925","HtmlAgilityPack" ),
                new CreditsMenuItem("http://www.telerik.com/products/windows-phone.aspx","Telerik" ),
                new CreditsMenuItem("http://coding4fun.codeplex.com/","Coding4Fun Toolkit" ),
                new CreditsMenuItem("http://www.windowsphone.com/en-US/store/publishers?publisherId=taropuff","taropuff", "Thanks for the images." ),
            };
        
            CreditsMenuItems = new ObservableCollection<CreditsMenuItem>(creditsMenuItems);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class AboutMenuItem
    {
        public string UniqueId { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public AboutMenuItem()
        {

        }

        public AboutMenuItem(string uniqueId, string header, string image)
        {
            UniqueId = uniqueId;
            Header = header;
            Image = image;
        }

        public AboutMenuItem(string uniqueId, string header, string description, string image)
        {
            UniqueId = uniqueId;
            Header = header;
            Description = description;
            Image = image;
        }
    }

    public class HistoryMenuItem 
    {
        public string Title { get; set; }
        public List<string> Updates { get; set; }

        public HistoryMenuItem()
        {

        }

        public HistoryMenuItem(string title, params string[] updates)
        {
            Title = title;
            Updates = new List<string>(updates);
        }
    }

    public class CreditsMenuItem
    {
        public string Url { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }

        public CreditsMenuItem()
        {

        }

        public CreditsMenuItem(string url, string header)
        {
            Url = url;
            Header = header;
        }

        public CreditsMenuItem(string url, string header, string description)
        {
            Url = url;
            Header = header;
            Description = description;
        }
    }
}
