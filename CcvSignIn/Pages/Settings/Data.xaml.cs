﻿using System;
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
            HomeViewModel.Instance.ClearChildren();
        }

        private void ClearSignInDataButton_Click(object sender, RoutedEventArgs e)
        {
            HomeViewModel.Instance.ClearSignIns();
        }
    }
}
