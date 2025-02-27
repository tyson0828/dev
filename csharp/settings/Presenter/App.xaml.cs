using System;
using System.IO;
using System.Windows;

namespace Presenter
{
    public partial class App : Application
    {
        private string logFilePath = "log.txt"; // Path to log file

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        // Handles UI Thread Exceptions
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogError("Unhandled UI Exception: " + e.Exception.Message);
            MessageBox.Show("An unexpected error occurred:\n" + e.Exception.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true; // Prevents crash
        }

        // Handles Non-UI Thread Exceptions
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogError("Unhandled Non-UI Exception: " + ex.Message);
            }
        }

        // Logs errors to file
        private void LogError(string message)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
            }
            catch
            {
                MessageBox.Show("Failed to write to log file.", "Logging Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
