﻿using CcvSignIn.Model;
using CcvSignIn.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace CcvSignIn.Pages
{
    class HomeViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HomeViewModel));

        #region Fields

        private static HomeViewModel _instance = new HomeViewModel();   // Singleton (don't judge me!)

        private string filterValue;
        private List<Child> children;
        private Child selectedChild;
        private Room selectedRoom;
        private PrintService printService = PrintService.Instance;
        private string dataFilename;

        #endregion

        #region Properties

        public static HomeViewModel Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets or sets the list of children to display.
        /// </summary>
        public List<Child> Children
        {
            get 
            { 
                if (!string.IsNullOrEmpty(filterValue))
                {
                    var filter = filterValue.Trim().ToLower();
                    var parts = filter.Split(' ');
                    if (parts.Length == 2)
                    {
                        // Special case - assume user typed first name and last name
                        var first = parts[0];
                        var last = parts[1];

                        return (
                            from c in children
                            where
                                c.First.ToLower().Contains(first)
                                && c.Last.ToLower().Contains(last)
                            orderby c.Last, c.First
                            select c
                        ).ToList<Child>();
                    }
                    else
                    {
                        return (
                            from c in children
                            where 
                                c.First.ToLower().Contains(filter)
                                || c.Last.ToLower().Contains(filter)
                            orderby c.Last, c.First
                            select c
                        ).ToList<Child>();
                    }
                }

                return children == null ? null : children.OrderBy(c => c.Last).ToList<Child>(); 
            }
            set
            {
                children = value;
                NotifyPropertyChanged("Children");
                NotifyPropertyChanged("NewcomerButtonEnabled");

                SelectedChild = null;
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

                SelectedChild = null;

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
                NotifyPropertyChanged("UpdateContactDetailsVisibility");
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
        /// Gets the full list of rooms.
        /// </summary>
        public Collection<Room> Rooms { get; private set; }

        /// <summary>
        /// Gets the list of available rooms.
        /// </summary>
        public List<Room> AvailableRooms
        {
            get { return Rooms.Where(r => r.IsAvailable).ToList(); }
        }

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

        /// <summary>
        /// Gets the path to the data file currently in use.
        /// </summary>
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

        /// <summary>
        /// Gets the visibility of the update contact details warning.
        /// </summary>
        public Visibility UpdateContactDetailsVisibility
        {
            get
            {
                if (selectedChild != null && selectedChild.UpdateRequired) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets a flag to indicate whether data should be synchronised to the server
        /// </summary>
        public Boolean SyncServer { get; set; }

        #endregion

        #region Constructor

        private HomeViewModel()
        {
            SyncServer = true;
            Rooms = new Collection<Room>();
            XmlSerializer xs = new XmlSerializer(typeof(Collection<Room>));
            using (var reader = new StreamReader(@"Rooms.xml"))
            {
                var rooms = (Collection<Room>)xs.Deserialize(reader);
                foreach (var room in rooms) Rooms.Add(room);
            }
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

                    var id = Properties.Settings.Default.NextId;
                    var laptopLabel = Properties.Settings.Default.LaptopLabel;
                    var label = string.Format("{0}{1:D2}", laptopLabel, id);

                    Properties.Settings.Default.NextId += 1;
                    Properties.Settings.Default.Save();

                    logger.InfoFormat(
                        "Signed in {0} to {1} with label {2}",
                        selectedChild.Fullname,
                        SelectedRoom.Title,
                        label);

                    selectedChild.Room = SelectedRoom.Title;
                    selectedChild.RoomLabel = SelectedRoom.ShowOnLabel ? SelectedRoom.Title : "";
                    selectedChild.SignedInAt = DateTime.Now;
                    selectedChild.Label = label;

                    printService.Print(selectedChild);
                }
                else if (selectedRoom.Title == SelectedChild.Room)
                {
                    // Clear details

                    logger.InfoFormat(
                        "Cleared details for {0} ({1}), was signed into {2}",
                        selectedChild.Fullname,
                        selectedChild.Label,
                        SelectedChild.Room);

                    selectedChild.Room = null;
                    selectedChild.RoomLabel = null;
                    selectedChild.SignedInAt = null;
                    selectedChild.Label = null;
                    selectedRoom = null;
                }
                else if (selectedRoom.Title != SelectedChild.Room)
                {
                    // Change sign in room

                    logger.InfoFormat(
                        "Changed sign in details for {0} ({1}) from {2} to {3}",
                        selectedChild.Fullname,
                        selectedChild.Label,
                        SelectedChild.Room,
                        SelectedRoom.Title);

                    selectedChild.Room = SelectedRoom.Title;
                    selectedChild.RoomLabel = SelectedRoom.ShowOnLabel ? SelectedRoom.Title : "";
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
            children.Add(new Child
                {
                    First = first,
                    Last = last,
                    IsNewcomer = true,
                });

            new CsvService().SaveData(DataFilename, children);

            if (FilterValue == last) NotifyPropertyChanged("Children");
            else FilterValue = last;

            SelectedChild = children.FirstOrDefault(c => c.First == first && c.Last == last && c.IsNewcomer);
        }

        /// <summary>
        /// Clears down the list of children.
        /// </summary>
        public void ClearChildren()
        {
            Children = new List<Child>();
            new CsvService().SaveData(DataFilename, children);
        }

        /// <summary>
        /// Clears down the sign ins but leaves the children.
        /// </summary>
        public void ClearSignIns()
        {
            foreach (var child in children.Where(c => c.SignedInAt != null))
            {
                child.Room = null;
                child.RoomLabel = null;
                child.SignedInAt = null;
                child.Label = null;
            }

            new CsvService().SaveData(DataFilename, children);

            NotifyPropertyChanged("Children");

            SelectedChild = null;
        }

        public void SaveFile()
        {
            new CsvService().SaveData(DataFilename, children);
        }
    }
}
