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
    /// Interaction logic for Newcomers.xaml
    /// </summary>
    public partial class Newcomers : UserControl
    {
        public Newcomers()
        {
            InitializeComponent();
            DataContext = new NewcomersViewModel();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = ((NewcomersViewModel)DataContext);
        }

        
    }
}
