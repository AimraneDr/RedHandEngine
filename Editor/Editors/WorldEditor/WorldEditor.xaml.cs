using Editor.GameDev;
using Editor.GameProjectFiles;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace Editor.Editors
{
    /// <summary>
    /// Interaction logic for WorldEditor.xaml
    /// </summary>
    public partial class WorldEditor : UserControl
    {
        public WorldEditor()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            this.Focus();
            //((INotifyCollectionChanged)Project.UndoRedo.UndoActionsList).CollectionChanged += (s, e) => Focus();
        }

        private void OnNewScriptBtn_Click(object sender, RoutedEventArgs e)
        {
           new NewScriptDialog().ShowDialog();
        }
    }
}
