using Editor.GameProjectFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string EnginePath { get; internal set; } = @"D:\VS_Projects\RedHandEngine";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
            Closing += MainWindowClosing;
        }

        void OpenProjectBrowserDialog()
        {
            var browserDialog = new ProjectBrowser();
            if (browserDialog.ShowDialog() == false || browserDialog.DataContext == null)
            {
                App.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                DataContext = browserDialog.DataContext;
            }
        }

        private void GetEnginePath()
        {
            var enginePath = Environment.GetEnvironmentVariable("REDHAND_ENGINE", EnvironmentVariableTarget.User);
            if(enginePath == null || !Directory.Exists(Path.Combine(enginePath,@"RedHandEngine\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if(dlg.ShowDialog() == true)
                {
                    EnginePath = dlg.EnginePath;
                    Environment.SetEnvironmentVariable("REDHAND_ENGINE", EnginePath.ToUpper(),EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                EnginePath = enginePath;
            }
        }


        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            Closing-= MainWindowClosing;
            Project.Current?.Unload();
        }
    }
}
