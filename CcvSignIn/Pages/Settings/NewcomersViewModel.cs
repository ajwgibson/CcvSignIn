using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcvSignIn.Pages.Settings
{
    public class NewcomersViewModel : ViewModelBase
    {
        int nextId = Properties.Settings.Default.NextNewcomerId;

        public int NextId 
        {
            get { return nextId; }
            set
            {
                if (nextId == value) return;
                nextId = value;

                Properties.Settings.Default.NextNewcomerId = nextId;
                Properties.Settings.Default.Save();

                NotifyPropertyChanged("NextId");
            }
        }
    }
}
