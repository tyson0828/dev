using System.Windows.Controls;

namespace Presenter.Views
{
    public partial class DefeatureSettingsView : UserControl
    {
        public DefeatureSettingsView()
        {
            InitializeComponent();
            this.DataContext = new Presenter.ViewModels.DefeatureSettingsViewModel();
        }
    }
}
