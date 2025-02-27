using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Linq;

namespace DynamicXMLConfig
{
    public partial class Defeature : Window
    {
        private string xmlFilePath = "config.xml"; // XML configuration file
        private XElement configXml; // Store XML content
        private string logFilePath = "log.txt"; // Log file for errors

        public Defeature()
        {
            InitializeComponent();
            LoadConfiguration();
        }

        private void LoadConfiguration(object sender, RoutedEventArgs e)  // ✅ Correct signature
        {
            LoadConfiguration(); // Calls the existing function without parameters
        }

        private void LoadConfiguration()
        {
            try
            {
                // Load XML File
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException($"Configuration file '{xmlFilePath}' not found.");
                }

                configXml = XElement.Load(xmlFilePath);
                StackPanelConfig.Children.Clear(); // Clear old UI elements

                XElement defeature = configXml.Element("Defeature");
                if (defeature == null)
                {
                    throw new Exception("Missing 'Defeature' section in XML.");
                }

                foreach (var token in defeature.Elements())
                {
                    string tokenName = token.Name.LocalName;
                    string prefixOffset = token.Attribute("PrefixOffset")?.Value ?? "0";

                    // Create Expander for collapsible section
                    Expander tokenExpander = new Expander
                    {
                        Header = tokenName,
                        IsExpanded = true,
                        Margin = new Thickness(5)
                    };

                    StackPanel tokenPanel = new StackPanel { Margin = new Thickness(10) };
                    tokenExpander.Content = tokenPanel;

                    // PrefixOffset (TextBox)
                    Label lblPrefix = new Label { Content = "PrefixOffset", Margin = new Thickness(5) };
                    TextBox txtPrefix = new TextBox
                    {
                        Text = prefixOffset,
                        Margin = new Thickness(5),
                        Tag = token
                    };
                    txtPrefix.TextChanged += UpdateXml;

                    tokenPanel.Children.Add(lblPrefix);
                    tokenPanel.Children.Add(txtPrefix);

                    // Add SQLSearchKey List
                    Label lblSQL = new Label { Content = "SQLSearchKeys:", Margin = new Thickness(5) };
                    tokenPanel.Children.Add(lblSQL);

                    StackPanel sqlListPanel = new StackPanel();
                    foreach (var sqlKey in token.Elements("SQLSearchKey"))
                    {
                        AddSQLSearchKeyUI(sqlListPanel, sqlKey, token);
                    }

                    // TextBox and Button for Adding SQLSearchKey
                    TextBox newSQLKeyBox = new TextBox { Width = 200, Margin = new Thickness(5) };
                    Button addSQLKeyButton = new Button
                    {
                        Content = "Add SQLSearchKey",
                        Margin = new Thickness(5)
                    };
                    addSQLKeyButton.Click += (sender, e) => AddNewSQLSearchKey(newSQLKeyBox, sqlListPanel, token);

                    StackPanel addNewSQLPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    addNewSQLPanel.Children.Add(newSQLKeyBox);
                    addNewSQLPanel.Children.Add(addSQLKeyButton);

                    tokenPanel.Children.Add(sqlListPanel);
                    tokenPanel.Children.Add(addNewSQLPanel);

                    StackPanelConfig.Children.Add(tokenExpander);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                MessageBox.Show($"Error loading configuration:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddSQLSearchKeyUI(StackPanel parentPanel, XElement sqlKeyElement, XElement parentToken)
        {
            StackPanel sqlRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

            TextBox sqlTextBox = new TextBox
            {
                Text = sqlKeyElement.Value,
                Width = 200,
                Margin = new Thickness(5),
                Tag = sqlKeyElement
            };
            sqlTextBox.TextChanged += UpdateXml;

            Button removeButton = new Button
            {
                Content = "Remove",
                Margin = new Thickness(5)
            };
            removeButton.Click += (sender, e) => RemoveSQLSearchKey(sqlRow, sqlKeyElement, parentToken);

            sqlRow.Children.Add(sqlTextBox);
            sqlRow.Children.Add(removeButton);

            parentPanel.Children.Add(sqlRow);
        }

        private void AddNewSQLSearchKey(TextBox inputBox, StackPanel sqlListPanel, XElement parentToken)
        {
            if (string.IsNullOrWhiteSpace(inputBox.Text))
            {
                MessageBox.Show("SQLSearchKey cannot be empty.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            XElement newSQLKey = new XElement("SQLSearchKey", inputBox.Text);
            parentToken.Add(newSQLKey);
            configXml.Save(xmlFilePath);

            AddSQLSearchKeyUI(sqlListPanel, newSQLKey, parentToken);
            inputBox.Clear();
        }

        private void RemoveSQLSearchKey(StackPanel rowPanel, XElement sqlKeyElement, XElement parentToken)
        {
            parentToken.Elements("SQLSearchKey").Where(e => e.Value == sqlKeyElement.Value).Remove();
            configXml.Save(xmlFilePath);
            StackPanelConfig.Children.Remove(rowPanel);
        }

        private void UpdateXml(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Control control)
                {
                    if (control.Tag is XElement element)
                    {
                        if (control is TextBox textBox)
                        {
                            element.Value = textBox.Text;
                        }
                        configXml.Save(xmlFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to update XML: {ex.Message}");
                MessageBox.Show("Failed to update XML. Please check the log for details.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

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