using Editor.Components;
using Editor.GameProjectFiles;
using Editor.Utilities;
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

namespace Editor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private string _propertyName;
        private Action _undoAction;
        public static GameEntityView Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;

            DataContextChanged += (_, __) =>
            {
                if(DataContext != null)
                {
                    (DataContext as MultiSelectionEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }
        private Action GetRenameAction()
        {
            var mse = DataContext as MultiSelectionEntity;
            var selection = mse.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MultiSelectionEntity).Refresh();
            });
        }
        private Action GetIsEnabledAction()
        {
            var mse = DataContext as MultiSelectionEntity;
            var selection = mse.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                (DataContext as MultiSelectionEntity).Refresh();
            });
        }
        private void OnName_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_propertyName == nameof(MultiSelectionEntity.Name) && _undoAction != null)
            {
                var _redoAction = GetRenameAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, _redoAction, "Rename Game Enity(ies)"));
                _propertyName = null;
            }
            _undoAction = null;
        }

        private void OnName_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _propertyName = string.Empty;
            _undoAction = GetRenameAction();
        }

        private void OnIsEnabled_Btn_Click(object sender, RoutedEventArgs e)
        {
            var undoAction = GetIsEnabledAction();
            var mse = DataContext as MultiSelectionEntity;
            mse.IsEnabled = (sender as CheckBox).IsChecked == true;
            var redoAction = GetIsEnabledAction();
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction,
                mse.IsEnabled == true ? "Enable Game Entity(ies)" : "Disable Game Entity(ies)"));
        }
    }
}
