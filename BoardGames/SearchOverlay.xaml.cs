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

namespace BoardGames
{
    public partial class SearchOverlay : UserControl
    {
        public event RoutedEventHandler SearchBoxLostFocus;
        public event KeyEventHandler SearchBoxKeyDown;

        public SearchOverlay()
        {
            InitializeComponent();
            SearchBox.GotFocus += SearchBox_GotFocus;
            SearchBox.LostFocus += SearchBox_LostFocus;
            SearchBox.KeyDown += SearchBox_KeyDown;
        }

        void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.SelectionStart = 0;
            SearchBox.SelectionLength = SearchBox.Text.Length;
        }

        public void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var eventHandler = this.SearchBoxLostFocus;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var eventHandler = this.SearchBoxKeyDown;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
    }
}
