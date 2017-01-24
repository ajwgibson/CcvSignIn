namespace CcvSignIn.Pages.Settings
{
    public class GeneralViewModel : ViewModelBase
    {
        private int    nextId      = Properties.Settings.Default.NextId;
        private string laptopLabel = Properties.Settings.Default.LaptopLabel;
        private string apiBaseUrl  = Properties.Settings.Default.ApiBaseUrl;

        public int NextId 
        {
            get { return nextId; }
            set { nextId = value; }
        }

        public string LaptopLabel
        {
            get { return laptopLabel; }
            set { laptopLabel = value; }
        }

        public string ApiBaseUrl
        {
            get { return apiBaseUrl; }
            set { apiBaseUrl = value; }
        }

        public void Refresh()
        {
            NextId      = Properties.Settings.Default.NextId;
            LaptopLabel = Properties.Settings.Default.LaptopLabel;
            ApiBaseUrl  = Properties.Settings.Default.ApiBaseUrl;
            NotifyPropertyChanged("NextId");
            NotifyPropertyChanged("LaptopLabel");
            NotifyPropertyChanged("ApiBaseUrl");
        }

        public void Save()
        {
            Properties.Settings.Default.NextId      = nextId;
            Properties.Settings.Default.LaptopLabel = laptopLabel;
            Properties.Settings.Default.ApiBaseUrl  = apiBaseUrl;
            Properties.Settings.Default.Save();
        }
    }
}
