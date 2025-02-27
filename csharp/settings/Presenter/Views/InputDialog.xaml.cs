using System.Windows;

namespace Presenter.Views
{
    public partial class InputDialog : Window
    {
        public string ResponseText { get; private set; }

        public InputDialog(string message)
        {
            InitializeComponent();
            txtResponse.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = txtResponse.Text;
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
