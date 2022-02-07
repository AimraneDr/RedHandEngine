using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor.GameProjectFiles
{
    /// <summary>
    /// Interaction logic for OpenProject.xaml
    /// </summary>
    public partial class OpenExistsProject : UserControl
    {
        public OpenExistsProject()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var li = ProjectsListBox.ItemContainerGenerator.ContainerFromIndex(ProjectsListBox.SelectedIndex) as ListItem;
                li?.Focus();
            };
        }
        void OpenSelectedProject()
        {
            var project = OpenProject.Open(ProjectsListBox.SelectedItem as ProjectData);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (project != null)
            {
                dialogResult = true;
                win.DataContext = project;
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedProject();
        }
    }
}
