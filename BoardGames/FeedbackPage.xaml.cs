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
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using BoardGames.Resources;

namespace BoardGames
{
    public partial class FeedbackPage : PhoneApplicationPage
    {
        private FeedbackViewModel model;

        public FeedbackPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            model = new FeedbackViewModel();
            DataContext = model;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton emailAppBarButton =
                new ApplicationBarIconButton(new
                Uri("/Assets/AppBar/feature.email.png", UriKind.Relative));
            emailAppBarButton.Text = AppResources.FeedbackEmailButtonText;
            emailAppBarButton.Click += EmailButton_Click;
            ApplicationBar.Buttons.Add(emailAppBarButton);

            ApplicationBarIconButton cancelAppBarButton =
                new ApplicationBarIconButton(new
                Uri("/Assets/AppBar/cancel.png", UriKind.Relative));
            cancelAppBarButton.Text = AppResources.FeedbackCancelButtonText;
            cancelAppBarButton.Click += CancelButton_Click;
            ApplicationBar.Buttons.Add(cancelAppBarButton);
        }

        private void FeedbackTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FeedbackTextBox.Text == FeedbackViewModel.DEFAULT_FEEDBACK)
            {
                FeedbackTextBox.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                FeedbackTextBox.Foreground = brush;
            }
        }

        private void FeedbackTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FeedbackTextBox.Text == String.Empty)
            {
                FeedbackTextBox.Text = FeedbackViewModel.DEFAULT_FEEDBACK;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                FeedbackTextBox.Foreground = brush;
            }
        }

        private void EmailButton_Click(object sender, EventArgs e)
        {
            var listPickerItem = FeedbackPicker.SelectedItem as ListPickerItem;

            CustomMessageBox feedbackMessageBox = new CustomMessageBox()
            {
                Caption = "User Feedback",
                Message = "Please select a feedback option from the list before sending.",
                LeftButtonContent = "ok"
            };

            if (FeedbackPicker.SelectedIndex == 0)
            {
                feedbackMessageBox.Show();
                return;
            }

            CustomMessageBox messageMessageBox = new CustomMessageBox()
            {
                Caption = "User Feedback",
                Message = "Please add a message, suggestion or question to the developer before sending.",
                LeftButtonContent = "ok"
            };

            if (FeedbackTextBox.Text == FeedbackViewModel.DEFAULT_FEEDBACK)
            {
                messageMessageBox.Show();
                return;
            }

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            Random random = new Random();
            int randomNumber = random.Next(100000, 1000000);

            emailComposeTask.To = "2pawns@outlook.com";
            emailComposeTask.Subject = String.Format("Windows Phone App Feedback: {0} ({1})", "BGDb", randomNumber);
            emailComposeTask.Body = String.Format("Feedback Type:\n{0}\n\nUser Message:\n{1}", listPickerItem.Content, FeedbackTextBox.Text);
            
            emailComposeTask.Show();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}