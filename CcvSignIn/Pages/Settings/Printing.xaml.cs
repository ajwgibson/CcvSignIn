using CcvSignIn.Model;
using CcvSignIn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CcvSignIn.Pages.Settings
{
    /// <summary>
    /// Interaction logic for Printing.xaml
    /// </summary>
    public partial class Printing : UserControl
    {
        public Printing()
        {
            InitializeComponent();
            DataContext = new PrintingViewModel();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = ((PrintingViewModel)DataContext);
            PrintService.Instance.Configure(context.Printer, context.LabelFile, context.Copies);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".label";
            dlg.Filter = "Dymo label files (.label)|*.label";
            dlg.FileName = ((PrintingViewModel)DataContext).LabelFile;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ((PrintingViewModel)DataContext).LabelFile = dlg.FileName;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            PrintService.Instance.Print(new Child { First = "Test", Last = "Person", Room = "A room", IsNewcomer = true }, 1);
        }
    }
}
