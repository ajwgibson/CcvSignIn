using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;

namespace CcvSignIn.Pages.Settings
{
    /// <summary>
    /// Interaction logic for ServerLogin.xaml
    /// </summary>
    public partial class ServerLogin : ModernDialog
    {
        public ServerLogin()
        {
            InitializeComponent();
            DataContext = new ServerLoginViewModel();
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }
    }
}
