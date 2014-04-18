using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace BoardGames
{
    public partial class BoardGamesHeader : UserControl
    {
        //public static readonly DependencyProperty SearchButtonTextVisibilityProperty =
        //    DependencyProperty.Register("SearchButtonTextVisibility", typeof(Visibility), typeof(BoardGamesHeader), new PropertyMetadata(default(Visibility), TitlePropertyChanged));

        //private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BoardGamesHeader myControl = d as BoardGamesHeader;
        //    myControl.SearchButtonTextBlock.Visibility = (Visibility)e.NewValue;
        //}

        //public Visibility SearchButtonTextVisibility
        //{
        //    get { return (Visibility)GetValue(SearchButtonTextVisibilityProperty); }
        //    set { SetValue(SearchButtonTextVisibilityProperty, value); }
        //}

        public static readonly DependencyProperty LoadingProperty =
            DependencyProperty.Register("Loading", typeof(Boolean), typeof(BoardGamesHeader), new PropertyMetadata(default(Boolean), LoadingPropertyChanged));

        private static void LoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BoardGamesHeader myControl = d as BoardGamesHeader;
            bool loading = (bool)e.NewValue;
            if (loading)
            {
                myControl.Spinner.SpinningAnimation.Begin();
                myControl.Spinner.Visibility = Visibility.Visible;
            }
            else
            {
                myControl.Spinner.Visibility = Visibility.Collapsed;
                myControl.Spinner.SpinningAnimation.Stop();
            }
        }

        public bool Loading
        {
            get
            {
                return (bool)GetValue(LoadingProperty);
            }
            set
            {
                SetValue(LoadingProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(BoardGamesHeader), new PropertyMetadata(default(String), TitlePropertyChanged));

        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BoardGamesHeader myControl = d as BoardGamesHeader;
            string title = (string)e.NewValue;
            myControl.TitleButtonTextBlock.Text = title;
        }

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public event RoutedEventHandler SearchClicked;
        public Visibility SearchButtonTextVisibility
        {
            get
            {
                return SearchButtonTextBlock.Visibility;
            }
            set
            {
                SearchButtonTextBlock.Visibility = value;
            }
        }

        public BoardGamesHeader()
        {
            InitializeComponent();
            SearchButton.Click += SearchButton_Click;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var eventHandler = this.SearchClicked;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        private void BoardGameButton_Click(object sender, RoutedEventArgs e)
        {
            Page pg = GetDependencyObjectFromVisualTree(this, typeof(Page)) as Page;

            pg.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            DependencyObject parent = startObject;

            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                {
                    break;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
            return parent;
        }
    }
}
