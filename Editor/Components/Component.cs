using Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components
{
    interface IMultiSelectionComponent
    {

    }

    [DataContract]
    abstract class Component : ViewModelBase
    {
        [DataMember]
        public GameEntity Owner { get; private set; }
        public Component(GameEntity owner)
        {
            Owner = owner;
        }
        public abstract IMultiSelectionComponent GetMultiSelectionComponent(MultiSelectionEntity msEntity);
    }

    abstract class MultiSelectionComponent<T> : ViewModelBase, IMultiSelectionComponent where T : Component
    {
        private bool _enableUpdate = true;
        public GameEntity Owner { get; private set; }
        public List<T> SelectedComponents { get; }

        public MultiSelectionComponent(MultiSelectionEntity msEntity)
        {
            Debug.Assert(msEntity?.SelectedEntities?.Any() == true);
            SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();

            PropertyChanged += (s, e) => { if (_enableUpdate) UpdateComponents(e.PropertyName); };
        }

        public void Refresh()
        {
            _enableUpdate = false;
            UpdateMSComponent();
            _enableUpdate = true;
        }

        protected abstract bool UpdateComponents(string PropertyName);
        protected abstract bool UpdateMSComponent();
    }
}
