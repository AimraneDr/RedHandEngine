using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utilities
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }
    public class UndoRedo
    {
        private bool _enableAdd = true;
        private readonly ObservableCollection<IUndoRedo> undo_actions_list = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> redo_actions_list = new ObservableCollection<IUndoRedo>();
        public ReadOnlyObservableCollection<IUndoRedo> RedoActionsList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoActionsList { get; }

        public UndoRedo()
        {
            RedoActionsList = new ReadOnlyObservableCollection<IUndoRedo>(redo_actions_list);
            UndoActionsList = new ReadOnlyObservableCollection<IUndoRedo>(undo_actions_list);
        }

        public void Add(IUndoRedo cmd)
        {
            if (_enableAdd)
            {
                undo_actions_list.Add(cmd);
                redo_actions_list.Clear();
            }
        }
        public void Undo()
        {
            if (undo_actions_list.Any())
            {
                var cmd = undo_actions_list.Last();
                undo_actions_list.RemoveAt(undo_actions_list.Count - 1);
                _enableAdd = false;
                cmd.Undo();
                _enableAdd = true;
                redo_actions_list.Insert(0, cmd);
            }
        }
        public void Redo()
        {
            if (redo_actions_list.Any())
            {
                var cmd = redo_actions_list.First();
                redo_actions_list.RemoveAt(0);
                _enableAdd = false;
                cmd.Redo();
                _enableAdd = true;
                undo_actions_list.Add(cmd);
            }
        }

        public void Reset()
        {
            redo_actions_list.Clear();
            undo_actions_list.Clear();
        }

    }

    public class UndoRedoAction : IUndoRedo
    {
        public string Name { get; }
        private Action _undoAction, _redoAction;
        public UndoRedoAction(string name)
        {
            Name = name;
        }
        public UndoRedoAction(Action undo, Action redo, string name)
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
        }
        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string actionName) :
            this(
                () => instance.GetType().GetProperty(property).SetValue(instance,undoValue),
                () => instance.GetType().GetProperty(property).SetValue(instance, redoValue),
                actionName
                )
        { }

        public void Redo() => _redoAction();

        public void Undo() => _undoAction();
    }
}
