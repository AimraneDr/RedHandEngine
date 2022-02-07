using Editor.Components;
using Editor.GameProjectFiles;
using Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;


namespace Editor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        bool _propertyChanged = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private Action GetAction(Func<Transform,(Transform transform,Vector3)> selector,
            Action<(Transform transform, Vector3)> foreachAction)
        {
            if (!(DataContext is MultiSelectionTransform vm))
            {
                _propertyChanged = false;
                _undoAction = null;
                return null;
            }
            var selection = vm.SelectedComponents.Select(x=>selector(x)).ToList();
            return new Action(() =>
            {
                selection.ForEach(x=>foreachAction(x));
                (GameEntityView.Instance.DataContext as MultiSelectionEntity)?.GetMultiSelectionComponent<MultiSelectionTransform>().Refresh();
            });
        }

        Action GetPositionAction() => GetAction((x) => (x,x.Position),(x) => x.transform.Position = x.Item2);
        Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);
        Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

        /// <summary>
        /// Records The spicific Action to the UndoRedo Collection
        /// </summary>
        /// <param name="redoAction"></param>
        /// <param name="name"></param>
        public void RecordActions(Action redoAction,string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }
        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MultiSelectionTransform).PropertyChanged+=(s,e)=>_propertyChanged = true;
        }

        private void PositionBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }

        private void PositionBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetPositionAction(), "Postion Changed");
        }

        private void PositionBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyChanged && _undoAction != null)
            {
                PositionBox_PreviewMouseLeftButtonUp(sender, null);
            }
        }

        private void RotationBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetRotationAction();
        }

        private void RotationBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetRotationAction(), "Rotation Changed");
        }

        private void RotationBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                RotationBox_PreviewMouseLeftButtonUp(sender, null);
            }
        }

        private void ScaleBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetScaleAction();
        }

        private void ScaleBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetScaleAction(), "Scale Changed");
        }

        private void ScaleBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                ScaleBox_PreviewMouseLeftButtonUp(sender, null);
            }
        }
    }
}
