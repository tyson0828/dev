using System.Windows;
using System.Windows.Input;

namespace Presenter.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Visibility _defeatureSettingsVisibility = Visibility.Collapsed;

        public Visibility DefeatureSettingsVisibility
        {
            get => _defeatureSettingsVisibility;
            set
            {
                _defeatureSettingsVisibility = value;
                OnPropertyChanged(nameof(DefeatureSettingsVisibility));
            }
        }

        private object _defeatureSettingsView;
        public object DefeatureSettingsView
        {
            get
            {
                if (_defeatureSettingsView == null)
                    _defeatureSettingsView = new Presenter.Views.DefeatureSettingsView();
                return _defeatureSettingsView;
            }
        }

        public ICommand ClickCommand { get; }
        public ICommand OpenDefeatureSettingsCommand { get; }

        public SettingsViewModel()
        {
            ClickCommand = new RelayCommand(ToggleDefeatureSettingsVisibility);
            OpenDefeatureSettingsCommand = new RelayCommand(OpenDefeatureSettings);
        }

        private void ToggleDefeatureSettingsVisibility()
        {
            DefeatureSettingsVisibility = Visibility.Visible;
        }

        private void OpenDefeatureSettings()
        {
            DefeatureSettingsVisibility = Visibility.Visible;
        }
    }
}
