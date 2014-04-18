using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.ViewModels
{
    public class FeedbackViewModel : INotifyPropertyChanged
    {
        public static readonly string DEFAULT_FEEDBACK = "(type your message to the developer here)";

        private string feedback;
        public string Feedback {
            get
            {
                return feedback;
            }
            set
            {
                if (feedback != value)
                {
                    feedback = value;
                    NotifyPropertyChanged("Feedback");
                }
            }
        }

        public FeedbackViewModel()
        {
            Feedback = DEFAULT_FEEDBACK;
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
}
