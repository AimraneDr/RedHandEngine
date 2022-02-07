using System;
using System.Collections.Generic;
using System.IO;
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

namespace Editor
{
    /// <summary>
    /// Interaction logic for EnginePathDialog.xaml
    /// </summary>
    public partial class EnginePathDialog : Window
    {
        public string EnginePath { get; private set; }
        public EnginePathDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void Ok_Btn_Click(object sender, RoutedEventArgs e)
        {
            var path = pathTextBox.Text.Trim();
            messagesTextBlock.Text = string.Empty;

            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                messagesTextBlock.Text = "Need to Type a Project Name .";
            }
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                messagesTextBlock.Text = "Invalid Character(s) used in Project Path.";
            }
            else if (!Directory.Exists(Path.Combine(path, @"RedHandEngine\EngineAPI")))
            {
                messagesTextBlock.Text = "Unable to find the Engine files in the specified location !";
            }

            if (string.IsNullOrEmpty(messagesTextBlock.Text))
            {
                if (!Path.EndsInDirectorySeparator(path)) path += @"\";
                EnginePath = path;
                DialogResult = true;
                Close();
            }
        }
    }
}
