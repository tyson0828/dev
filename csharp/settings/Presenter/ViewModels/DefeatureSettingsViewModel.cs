using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Presenter.Models;

namespace Presenter.ViewModels
{
    public class DefeatureSettingsViewModel : ViewModelBase
    {
        private string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
        private XElement configXml;
        public ObservableCollection<DefeatureSettingsModel> DefeatureTokens { get; set; }

        public ICommand AddSQLSearchKeyCommand { get; }
        public ICommand RemoveSQLSearchKeyCommand { get; }
        public ICommand SaveCommand { get; }

        public DefeatureSettingsViewModel()
        {
            DefeatureTokens = new ObservableCollection<DefeatureSettingsModel>();
            LoadConfiguration();

            AddSQLSearchKeyCommand = new RelayCommand<object>(AddSQLSearchKey);
            RemoveSQLSearchKeyCommand = new RelayCommand<object>(RemoveSQLSearchKey);
            SaveCommand = new RelayCommand(SaveConfiguration);
        }

        private void LoadConfiguration()
        {
            try
            {
                if (!File.Exists(xmlFilePath))
                {
                    configXml = new XElement("Configuration", new XElement("Defeature"));
                    configXml.Save(xmlFilePath);
                }
                else
                {
                    configXml = XElement.Load(xmlFilePath);
                }

                DefeatureTokens.Clear();
                XElement defeature = configXml.Element("Defeature");
                if (defeature == null) return;

                foreach (var token in defeature.Elements())
                {
                    var model = new DefeatureSettingsModel
                    {
                        TokenCategory = token.Name.LocalName,
                        PrefixOffset = token.Attribute("PrefixOffset")?.Value ?? "0",
                        SQLSearchKeys = new ObservableCollection<string>(token.Elements("SQLSearchKey").Select(e => e.Value))
                    };
                    DefeatureTokens.Add(model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddSQLSearchKey(object parameter)
        {
            var inputDialog = new Presenter.Views.InputDialog("Enter new SQLSearchKey:");
            if (inputDialog.ShowDialog() == true)
            {
                string newKey = inputDialog.ResponseText;
                /*
                if (!string.IsNullOrWhiteSpace(newKey))
                {
                    DefeatureTokens.FirstOrDefault()?.SQLSearchKeys.Add(newKey);
                }
                */

                if (parameter is DefeatureSettingsModel model)
                {
                    model.SQLSearchKeys.Add(newKey);
                }
            }
        }

        private void RemoveSQLSearchKey(object parameter)
        {
          /*
            if (parameter is DefeatureSettingsModel model)
            {
                // If SQLSearchKeys is not empty, remove the last entry as an example
                if (model.SQLSearchKeys.Count > 0)
                {
                    model.SQLSearchKeys.RemoveAt(model.SQLSearchKeys.Count - 1);
                }
            }
            */

            if (parameter is Tuple<object, object> data)
            {
                string keyToRemove = data.Item1 as string; // SQLSearchKey
                var model = data.Item2 as DefeatureSettingsModel; // Corresponding Model

                if (model != null && !string.IsNullOrEmpty(keyToRemove) && model.SQLSearchKeys.Contains(keyToRemove))
                {
                    model.SQLSearchKeys.Remove(keyToRemove);
                }
            }
        }

/*
        private void RemoveSQLSearchKey(object parameter)
        {
            if (parameter is Tuple<DefeatureSettingsModel, string> data)
            {
                data.Item1.SQLSearchKeys.Remove(data.Item2);
            }
        }
*/
        private void SaveConfiguration()
        {
            try
            {
                if (configXml == null)
                {
                    configXml = new XElement("Configuration", new XElement("Defeature"));
                }

                XElement defeature = configXml.Element("Defeature");
                if (defeature == null)
                {
                    defeature = new XElement("Defeature");
                    configXml.Add(defeature);
                }

                defeature.RemoveAll();

                foreach (var token in DefeatureTokens)
                {
                    XElement tokenElement = new XElement("Token",
                        new XAttribute("PrefixOffset", token.PrefixOffset),
                        token.SQLSearchKeys.Select(key => new XElement("SQLSearchKey", key))
                    );
                    defeature.Add(tokenElement);
                }

                configXml.Save(xmlFilePath);
                MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
