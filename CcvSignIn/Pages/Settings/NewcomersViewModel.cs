using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcvSignIn.Pages.Settings
{
    public class NewcomersViewModel : ViewModelBase
    {
        private int nextId = Properties.Settings.Default.NextNewcomerId;

        public int NextId 
        {
            get { return nextId; }
            set { nextId = value; }
        }

        public void Refresh()
        {
            NextId = Properties.Settings.Default.NextNewcomerId;
            NotifyPropertyChanged("NextId");
        }

        public void Save()
        {
            Properties.Settings.Default.NextNewcomerId = nextId;
            Properties.Settings.Default.Save();
        }
    }
}
