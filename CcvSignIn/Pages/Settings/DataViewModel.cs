using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CcvSignIn.Pages.Settings
{
    public class DataViewModel : ViewModelBase
    {
        private Boolean syncServer = true;

        public string DataFilename
        {
            get { return HomeViewModel.Instance.DataFilename; }
            set
            {
                HomeViewModel.Instance.DataFilename = value;
                NotifyPropertyChanged("DataFilename");
                NotifyPropertyChanged("FileIsOpen");
            }
        }

        public Boolean SyncServer
        {
            get { return HomeViewModel.Instance.SyncServer; }
            set { HomeViewModel.Instance.SyncServer = value; }
        }

        public Visibility FileIsOpen
        {
            get
            {
                return string.IsNullOrEmpty(DataFilename) 
                    ? Visibility.Collapsed 
                    : Visibility.Visible;
            }
        }
    }
}
