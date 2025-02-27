using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Presenter.Models
{
    public class DefeatureSettingsModel : INotifyPropertyChanged
    {
        private string _prefixOffset;
        private string _tokenCategory;
        private ObservableCollection<string> _sqlSearchKeys;

        public string TokenCategory
        {
            get => _tokenCategory;
            set
            {
                _tokenCategory = value;
                OnPropertyChanged(nameof(TokenCategory));
            }
        }

        public string PrefixOffset
        {
            get => _prefixOffset;
            set
            {
                _prefixOffset = value;
                OnPropertyChanged(nameof(PrefixOffset));
            }
        }

        public ObservableCollection<string> SQLSearchKeys
        {
            get => _sqlSearchKeys;
            set
            {
                _sqlSearchKeys = value;
                OnPropertyChanged(nameof(SQLSearchKeys));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public DefeatureSettingsModel()
        {
            SQLSearchKeys = new ObservableCollection<string>();
        }
    }
}
