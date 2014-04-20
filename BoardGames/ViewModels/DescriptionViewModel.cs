using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.ViewModels
{
    public class DescriptionViewModel : INotifyPropertyChanged
    {
        private BoardGameCache cache;

        private string descriptionTitle;
        public string DescriptionTitle
        {
            get
            {
                return descriptionTitle;
            }
            set
            {
                if (descriptionTitle != value)
                {
                    descriptionTitle = value;
                    NotifyPropertyChanged("DescriptionTitle");
                }
            }
        }

        private string objectId;
        public string ObjectId
        {
            get
            {
                return objectId;
            }
            set
            {
                if (objectId != value)
                {
                    objectId = value;
                    NotifyPropertyChanged("ObjectId");
                }
            }
        }

        private BoardGame boardGame;
        public BoardGame BoardGame
        {
            get
            {
                return boardGame;
            }
            set
            {
                if (boardGame != value)
                {
                    boardGame = value;
                    NotifyPropertyChanged("BoardGame");
                }
            }
        }

        public DescriptionViewModel()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            cache = BoardGameCache.Instance;
            // Fake board game with temp title
            boardGame = new BoardGame()
            {
                Name = " "
            };
        }

        public async Task LoadBoardGameAsync()
        {
            if (objectId == null)
            {
                Debug.WriteLine("Attemted to load Board Game with no object id");
                return;
            }
            int id;
            if (int.TryParse(objectId, out id))
            {
                BoardGame = (await cache.BoardGamesFromIds(id)).First();
            }
            else
            {
                Debug.WriteLine(String.Format("Unable to parse id {0} ", objectId));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
