using System.Windows;

namespace Presenter.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            this.DataContext = new Presenter.ViewModels.SettingsViewModel();
        }
    }
}
