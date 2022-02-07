using Editor.DLLsHolder;
using Editor.GameProjectFiles;
using Editor.Utilities;
using Editor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
    {
        private int _entityID = ID.INVALID_ID;
        public int EntityID 
        {
            get => _entityID;
            private set
            {
                if(_entityID != value)
                {
                    _entityID = value;
                    OnPropertyChanged(nameof(EntityID));
                }
            }
        }

        private bool _isActive;
        public bool IsActive 
        {
            get => _isActive;
            set
            {
                if(_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        EntityID = EngineAPI.CreateGameEntity(this);
                        Debug.Assert(ID.IsValid(EntityID));
                    }
                    else if(ID.IsValid(EntityID))
                    {
                        EngineAPI.RemoveGameEntity(this);
                        EntityID = ID.INVALID_ID;
                    }
                    OnPropertyChanged(nameof(_isActive));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if(value != _name)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = "Components")]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }



        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));

            OnDeserialized(new StreamingContext());
        }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components)); 
            }
        }

        public Component GetComponent(Type type) => Components.First(c => c.GetType() == type);
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

    }

    abstract class MultiSelectionEntity : ViewModelBase
    {
        //Enable and Disable Updates for Selected E ntities
        private bool _enableUpdates = true;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool? _isEnabled = true;
        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private readonly ObservableCollection<IMultiSelectionComponent> _components = new ObservableCollection<IMultiSelectionComponent>();
        public ReadOnlyObservableCollection<IMultiSelectionComponent> Components { get; }

        public List<GameEntity> SelectedEntities { get; }

        public MultiSelectionEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMultiSelectionComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateGameEntities(e.PropertyName); };
        }
        
        public void Refresh()
        {
            _enableUpdates = false;
            UpdateGameEntity();
            FilterComponentsList();
            _enableUpdates = true;
        }

        private void FilterComponentsList()
        {
            _components.Clear();
            var first_entity = SelectedEntities.FirstOrDefault();
            if (first_entity == null) return;

            foreach(var component in first_entity.Components)
            {
                var type = component.GetType();
                if (!SelectedEntities.Skip(1).Any(entity => entity.GetComponent(type) == null))
                {
                    Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
                    _components.Add(component.GetMultiSelectionComponent(this));
                }
            }
        }

        protected virtual bool UpdateGameEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool?>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));
            return true;
        }
        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        public T GetMultiSelectionComponent<T>() where T : IMultiSelectionComponent
        {
            return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        #region GetMixedValue
        public static float? GetMixedValue<T>(List<T> objects,Func<T,float?> getPropertyValue)
        {
            var value = getPropertyValue(objects.First());
            return objects.Skip(1).Any(x => !getPropertyValue(x).IsSame(value)) ? (float?)null : value;
        }
        public static string GetMixedValue<T>(List<T> objects, Func<T, string> getPropertyValue)
        {
            var value = getPropertyValue(objects.First());
            return objects.Skip(1).Any(x => getPropertyValue(x) != value) ? (string)null : value;
        }
        public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool?> getPropertyValue)
        {
            var value = getPropertyValue(objects.First());
            return objects.Skip(1).Any(x => getPropertyValue(x) != value) ? (bool?)null : value;
        }
        #endregion

    }

    class MultiSelectionGameEntity : MultiSelectionEntity
    {
        public MultiSelectionGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
