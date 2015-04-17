using CcvSignIn.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CcvSignIn.Pages
{
    class HomeViewModel : INotifyPropertyChanged
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HomeViewModel));

        #region Fields

        private string filterValue;
        private List<Child> children;
        private Child selectedChild;
        private Room selectedRoom;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of children to display.
        /// </summary>
        public List<Child> Children
        {
            get 
            { 
                if (!string.IsNullOrEmpty(filterValue))
                {
                    return (
                        from c in children
                        where 
                            c.First.ToLower().Contains(filterValue.ToLower())
                            || c.Last.ToLower().Contains(filterValue.ToLower())
                        orderby c.Last, c.First
                        select c
                    ).ToList<Child>();
                }

                return children == null ? null : children.ToList<Child>(); 
            }
            set
            {
                children = value;
                NotifyPropertyChanged("Children");
            } 
        }

        /// <summary>
        /// Gets or sets the current filter value.
        /// </summary>
        public string FilterValue 
        { 
            get { return filterValue; }
            set
            {
                if (filterValue == value) return;
                filterValue = value;
                NotifyPropertyChanged("Children");
            }
        }

        /// <summary>
        /// Gets or sets the selected child.
        /// </summary>
        public Child SelectedChild 
        {
            get { return selectedChild;  }
            set
            {
                if (selectedChild == value) return;
                selectedChild = value;

                Rooms.ToList<Room>().ForEach(delegate(Room room) { room.IsSelected = false; });
                SelectedRoom = null;

                if (selectedChild != null && !string.IsNullOrEmpty(selectedChild.Room))
                {
                    SelectedRoom = Rooms.FirstOrDefault(r => r.Title == selectedChild.Room);
                }

                NotifyPropertyChanged("SelectedChild");
                NotifyPropertyChanged("SelectedChildVisibility");
            } 
        }

        
        /// <summary>
        /// Gets the visibility of the selected child.
        /// </summary>
        public Visibility SelectedChildVisibility 
        {
            get 
            {
                if (selectedChild != null) return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Gets the list of available rooms.
        /// </summary>
        public Collection<Room> Rooms { get; private set; }

        /// <summary>
        /// Gets the selected room.
        /// </summary>
        public Room SelectedRoom
        {
            get { return selectedRoom; }
            set
            {
                if (selectedRoom == value) return;
                selectedRoom = value;
                NotifyPropertyChanged("SelectedRoom");
                NotifyPropertyChanged("SignInEnabled");
            }
        }

        /// <summary>
        /// Is the sign in button enabled?
        /// </summary>
        public Boolean SignInEnabled
        {
            get { return SelectedRoom != null; }
        }

        #endregion

        #region Constructor

        public HomeViewModel()
        {
            Rooms = new Collection<Room>
                {
                    new Room { Title = "Tiny Stars",      Colour = "Pink",   Description = "9 months and over, not walking yet" },
                    new Room { Title = "Little Stars",    Colour = "Lime",   Description = "Walking but not at nursery yet" },
                    new Room { Title = "Small Stars",     Colour = "Blue",   Description = "Nursery & P1" },
                    new Room { Title = "Allstars Junior", Colour = "Green",  Description = "P2 & P3" },
                    new Room { Title = "Allstars",        Colour = "Orange", Description = "P3 & P4" },
                    new Room { Title = "Allstars High",   Colour = "Purple", Description = "P5, P6 & P7" },
                };
        }

        #endregion

        /// <summary>
        /// Perform a sign in action.
        /// </summary>
        public void SignIn()
        {
            if (selectedChild != null && SelectedRoom != null)
            {
                selectedChild.Room = SelectedRoom.Title;
                selectedChild.SignedInAt = DateTime.Now;

                logger.InfoFormat(
                    "Signed in {0} {1} ({2}) to {3}",
                    selectedChild.First,
                    selectedChild.Last,
                    selectedChild.Id,
                    SelectedRoom.Title);

                new CsvService().SaveOutputData(CcvSignIn.Properties.Settings.Default.OutputFilename, children);

                FireChangeEvents();
            }
        }

        #region Utility Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            var eventToFire = PropertyChanged;
            if (eventToFire != null)
                eventToFire(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FireChangeEvents()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(""));
        }

        #endregion

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
