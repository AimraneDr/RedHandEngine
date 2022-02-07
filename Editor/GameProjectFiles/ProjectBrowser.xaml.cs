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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Editor.GameProjectFiles
{
    /// <summary>
    /// Interaction logic for ProjectBrowser.xaml
    /// </summary>
    public partial class ProjectBrowser : Window
    {

        private readonly CubicEase easingFunc = new CubicEase() { EasingMode = EasingMode.EaseInOut };
        public ProjectBrowser()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserLoaded;
        }

        private void OnProjectBrowserLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserLoaded;
            if (!OpenProject.Projects.Any())
            {
                OpenProjectBtn.IsEnabled = false;
                OpenProjectView.Visibility = Visibility.Hidden;
                tToggleBtn_Click(CreateProjectBtn, new RoutedEventArgs());
            }
        }

        private void tToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender == OpenProjectBtn)
            {
                if (CreateProjectBtn.IsChecked == true)
                {
                    CreateProjectBtn.IsChecked = false;
                    AnimateToOenProject();
                    OpenProjectView.IsEnabled = true;
                    CreateProjectView.IsEnabled = false;
                }
                OpenProjectBtn.IsChecked = true;
            }
            else
            {
                if (OpenProjectBtn.IsChecked == true)
                {
                    OpenProjectBtn.IsChecked = false;
                    AnimateToCreateProject();
                    OpenProjectView.IsEnabled = false;
                    CreateProjectView.IsEnabled = true;
                }
                CreateProjectBtn.IsChecked = true;
            }
        }

        private void AnimateToCreateProject()
        {
            var high_light_animation = new DoubleAnimation(0, 72, new Duration(TimeSpan.FromSeconds(0.3)));
            high_light_animation.EasingFunction = easingFunc;
            high_light_animation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(0), new Thickness(0, -800, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = easingFunc;
                OpenCraeteProjectPanelsContainer.BeginAnimation(MarginProperty, animation);
            };
            HighLightRec.BeginAnimation(Canvas.TopProperty, high_light_animation);
        }

        private void AnimateToOenProject()
        {
            var high_light_animation = new DoubleAnimation(72, 0, new Duration(TimeSpan.FromSeconds(0.3)));
            high_light_animation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(0, -800, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = easingFunc;
                OpenCraeteProjectPanelsContainer.BeginAnimation(MarginProperty, animation);
            };
            high_light_animation.EasingFunction = easingFunc;
            HighLightRec.BeginAnimation(Canvas.TopProperty, high_light_animation);
        }
    }
}
