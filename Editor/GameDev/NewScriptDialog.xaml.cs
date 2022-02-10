using Editor.GameProjectFiles;
using Editor.Utilities.Logs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Editor.GameDev
{
    /// <summary>
    /// Interaction logic for NewScriptDialog.xaml
    /// </summary>
    public partial class NewScriptDialog : Window
    {
        public NewScriptDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            ScriptPath.Text = Project.ProjectGameScriptsFolderName;
        }

        bool Validate()
        {
            bool isValid = false;
            var name = ScriptName.Text.Trim();
            var path = ScriptPath.Text;
            string errorMsg = string.Empty;
            string pth = Project.Current.Path;
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                errorMsg = "Need to Type a Script Name .";
            }
            else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                errorMsg = "Invalid Character(s) used in Script Name.";
            }
            else if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                errorMsg = "Need to Type a Path Location .";
            }
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                errorMsg = "Invalid Character(s) used in Script Path.";
            }else if(!Path.GetFullPath(Path.Combine(Project.Current.Path,path)).Contains(Path.Combine(Project.Current.Path, Project.ProjectGameScriptsFolderName)))
            {
                errorMsg = "Script path must be added to GameScripts folder or one og its sub folders .";
            }else if (File.Exists(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp")) ||
                File.Exists(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h")))
            {
                errorMsg = $"a script with the same name '{name}' already exists in this folder";
            }
            else
            {
                isValid = true;
            }

            if (!isValid)
            {
                messagesTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
            }
            else
            {
                messagesTextBlock.Foreground = FindResource("Editor.FontColorBrush") as Brush;
            }
            messagesTextBlock.Text = errorMsg;

            return isValid;
        }
        private void OnScriptName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Validate()) return;
            var name = ScriptName.Text.Trim();
            messagesTextBlock.Text = $"{name}.h & {name}.cpp will be added to {Project.Current.Name} .";
        }
        private void OnScriptPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private async void OnCreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            BusyAnimation.Opacity = 0;
            BusyAnimation.Visibility = Visibility.Visible;
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
            BusyAnimation.BeginAnimation(OpacityProperty, fadeIn);

            var name = ScriptName.Text.Trim();
            var path = Path.GetFullPath(Path.Combine(Project.Current.Path, ScriptPath.Text.Trim()));
            var solution = Project.Current.Solution;
            var projectname = Project.Current.Name;
            IsEnabled = false;
            try
            {
                await Task.Run(() => CreateScript(name, path, solution, projectname));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); ;
                Logger.Log(MessageType.Error, $"Faild to Craete '{name}.h' & '{name}.cpp' in '{Project.Current.Name}' at '{path}' : \n\t" + ex.Message);
            }
            finally
            {
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(500)));
                fadeOut.Completed += (s, e) =>
                  {
                      BusyAnimation.Opacity = 0;
                      BusyAnimation.Visibility = Visibility.Hidden;
                      Close();
                  };
                BusyAnimation.BeginAnimation(OpacityProperty, fadeOut);
            }
        }

        private void CreateScript(string name, string path, string solution, string projectname)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var _namespace = GetNameSpaceFromProjectName();
            var cpp = Path.GetFullPath(Path.Combine(path,$"{name}.cpp"));
            var h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

            using(var sw = File.CreateText(cpp))
            {
                sw.Write(string.Format(ScriptTeplateInfo.cpp_file, name, _namespace));
            }
            using (var sw = File.CreateText(h))
            {
                sw.Write(string.Format(ScriptTeplateInfo.h_file , name, _namespace));
            }
            string[] files = new string[] { cpp, h };

            for (int i = 0; i < 5; i++)
            {
                if (!VisualStudio.AddFilesToSolution(solution, projectname, files)) System.Threading.Thread.Sleep(10000);
                else break;
            }
        }

        private string GetNameSpaceFromProjectName()
        {
            var projectName = Project.Current.Name;
            projectName = projectName.Replace(' ', '_');
            return projectName;
        }
    }
}
