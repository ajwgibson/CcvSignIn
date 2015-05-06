using CcvSignIn.Model;
using CcvSignIn.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CcvSignIn.Pages
{
    class HomeViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HomeViewModel));

        #region Fields

        private string filterValue;
        private List<Child> children;
        private Child selectedChild;
        private Room selectedRoom;
        private PrintService printService = PrintService.Instance;
        private string dataFilename;

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

                return children == null ? null : children.OrderBy(c => c.Last).ToList<Child>(); 
            }
            set
            {
                children = value;
                NotifyPropertyChanged("Children");
                NotifyPropertyChanged("NewcomerButtonEnabled");
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
                NotifyPropertyChanged("FilterValue");
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
                return Visibility.Collapsed;
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
                NotifyPropertyChanged("SignInButtonText");
                
            }
        }
        
        /// <summary>
        /// Gets the text that should be displayed on the sign in button.
        /// </summary>
        public string SignInButtonText 
        {
            get
            {
                if (selectedChild != null && string.IsNullOrEmpty(selectedChild.Room)) return "Sign in";
                if (selectedRoom != null && selectedRoom.Title == SelectedChild.Room) return "Clear";
                if (selectedRoom != null && selectedRoom.Title != SelectedChild.Room) return "Change";
                return "Sign in";
            }
        }

        /// <summary>
        /// Is the sign in button enabled?
        /// </summary>
        public Boolean SignInEnabled
        {
            get { return SelectedRoom != null; }
        }

        /// <summary>
        /// Gets a flag to indicate if the newcomer button should be enabled.
        /// </summary>
        public bool NewcomerButtonEnabled
        {
            get { return children != null && children.Count > 0; }
        }

        public string DataFilename
        {
            get { return dataFilename; }
            set
            {
                if (dataFilename == value) return;
                dataFilename = value;
                NotifyPropertyChanged("DataFilename");
            }
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
                    new Room { Title = "Kids Serving",    Colour = "Any",    Description = "Kids under the age of 11 who are serving" },
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
                if (string.IsNullOrEmpty(SelectedChild.Room)) 
                {
                    // Normal sign in

                    logger.InfoFormat(
                        "Signed in {0} ({1}) to {2}",
                        selectedChild.Fullname,
                        selectedChild.Id,
                        SelectedRoom.Title);

                    selectedChild.Room = SelectedRoom.Title;
                    selectedChild.SignedInAt = DateTime.Now;

                    printService.Print(selectedChild);
                }
                else if (selectedRoom.Title == SelectedChild.Room)
                {
                    // Clear details

                    logger.InfoFormat(
                        "Cleared details for {0} ({1}), was signed into {2}",
                        selectedChild.Fullname,
                        selectedChild.Id,
                        SelectedChild.Room);

                    selectedChild.Room = null;
                    selectedChild.SignedInAt = null;
                    selectedRoom = null;
                }
                else if (selectedRoom.Title != SelectedChild.Room)
                {
                    // Change sign in room

                    logger.InfoFormat(
                        "Changed sign in details for {0} ({1}) from {2} to {3}",
                        selectedChild.Fullname,
                        selectedChild.Id,
                        SelectedChild.Room,
                        SelectedRoom.Title);

                    selectedChild.Room = SelectedRoom.Title;
                    selectedChild.SignedInAt = DateTime.Now;

                    printService.Print(selectedChild);
                }

                new CsvService().SaveData(DataFilename, children);

                NotifyPropertyChanged("SelectedRoom");
                NotifyPropertyChanged("SignInEnabled");
                NotifyPropertyChanged("SignInButtonText");
                NotifyPropertyChanged("Children");
            }
        }

        /// <summary>
        /// Adds a new child to the list and selects them to allow immediate sign in
        /// </summary>
        public void AddNewcomer(string first, string last)
        {
            var id = Properties.Settings.Default.NextNewcomerId;

            children.Add(new Child
                {
                    Id = id,
                    First = first,
                    Last = last,
                    IsNewcomer = true,
                });

            new CsvService().SaveData(DataFilename, children);

            Properties.Settings.Default.NextNewcomerId += 1;
            Properties.Settings.Default.Save();

            if (FilterValue == last) NotifyPropertyChanged("Children");
            else FilterValue = last;

            SelectedChild = children.FirstOrDefault(c => c.Id == id);
        }
    }
}
