using CcvSignIn.Service;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CcvSignIn.Pages.Settings
{
    /// <summary>
    /// Interaction logic for Data.xaml
    /// </summary>
    public partial class Data : UserControl
    {
        public Data()
        {
            InitializeComponent();
            DataContext = new DataViewModel();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                ((DataViewModel)DataContext).DataFilename = dlg.FileName;
                HomeViewModel.Instance.Children = new CsvService().LoadData(dlg.FileName);
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                ((DataViewModel)DataContext).DataFilename = dlg.FileName;
                new CsvService().SaveData(dlg.FileName, HomeViewModel.Instance.Children);
            }
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = DateTime.Today.ToString("yyyy-MM-dd");
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                HomeViewModel.Instance.ClearChildren();
                ((DataViewModel)DataContext).DataFilename = dlg.FileName;
                new CsvService().SaveData(dlg.FileName, HomeViewModel.Instance.Children);
            }
        }

        private void ClearAllDataButton_Click(object sender, RoutedEventArgs e)
        {
            var result = ModernDialog.ShowMessage("Are you sure you want to clear all the data?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) HomeViewModel.Instance.ClearChildren();
        }

        private void ClearSignInDataButton_Click(object sender, RoutedEventArgs e)
        {
            var result = ModernDialog.ShowMessage("Are you sure you want to clear the sign in data?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) HomeViewModel.Instance.ClearSignIns();
        }

        private async void FetchServerDataButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ServerLogin { Title = "Login to server" };

            dlg.ShowDialog();

            var doLogin = dlg.DialogResult.HasValue ? (bool)dlg.DialogResult : false;
            var loginviewModel = (ServerLoginViewModel)dlg.DataContext;

            if (doLogin
                && !string.IsNullOrEmpty(loginviewModel.Username)
                && !string.IsNullOrEmpty(loginviewModel.ThePassword))
            {
                await ApiService.Login(loginviewModel.Username, loginviewModel.ThePassword);
                var result = await ApiService.DownloadChildren();
                HomeViewModel.Instance.Children = result;
                HomeViewModel.Instance.SaveFile();
            }
        }
    }
}
