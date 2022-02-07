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
    /// Interaction logic for CreateNewProject.xaml
    /// </summary>
    public partial class CreateNewProject : UserControl
    {
        public CreateNewProject()
        {
            InitializeComponent();
        }

        private void CraeteBtn_Click(object sender, RoutedEventArgs e)
        {
            var gp = DataContext as NewProject;
            var project_path = gp.CreateProject(TemplatesListBox.SelectedItem as ProjectTemplate);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (!string.IsNullOrEmpty(project_path))
            {
                dialogResult = true;
                var project = OpenProject.Open(new ProjectData() { ProjectName=gp.ProjectName, ProjectPath=project_path });
                win.DataContext = project;
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
