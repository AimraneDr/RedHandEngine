using Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components
{
    [DataContract]
    class Transform : Component
    {
        private Vector3 _position;
        [DataMember] 
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        private Vector3 _rotation;
        [DataMember] 
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if(value != _rotation)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        private Vector3 _scale;
        [DataMember]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        public Transform(GameEntity owner) : base(owner)
        {

        }

        public override IMultiSelectionComponent GetMultiSelectionComponent(MultiSelectionEntity msEntity)=>new MultiSelectionTransform(msEntity);
    }

    sealed class MultiSelectionTransform : MultiSelectionComponent<Transform>
    {
        private float? _posX;
        public float? posX
        {
            get => _posX;
            set
            {
                if (!MathUtl.IsSame(_posX, value))
                {
                    _posX = value;
                    OnPropertyChanged(nameof(posX));
                }
            }
        }
        private float? _posY;
        public float? posY
        {
            get => _posY;
            set
            {
                if (!MathUtl.IsSame(_posY, value))
                {
                    _posY = value;
                    OnPropertyChanged(nameof(posY));
                }
            }
        }
        private float? _posZ;
        public float? posZ
        {
            get => _posZ;
            set
            {
                if (!MathUtl.IsSame(_posZ, value))
                {
                    _posZ = value;
                    OnPropertyChanged(nameof(posZ));
                }
            }
        }
        private float? _rotX;
        public float? rotX
        {
            get => _rotX;
            set
            {
                if (!MathUtl.IsSame(_rotX, value))
                {
                    _rotX = value;
                    OnPropertyChanged(nameof(rotX));
                }
            }
        }
        private float? _rotY;
        public float? rotY
        {
            get => _rotY;
            set
            {
                if (!MathUtl.IsSame(_rotY, value))
                {
                    _rotY = value;
                    OnPropertyChanged(nameof(rotY));
                }
            }
        }
        private float? _rotZ;
        public float? rotZ
        {
            get => _rotZ;
            set
            {
                if (!MathUtl.IsSame(_rotZ, value))
                {
                    _rotZ = value;
                    OnPropertyChanged(nameof(rotZ));
                }
            }
        }
        private float? _scaleX;
        public float? scaleX
        {
            get => _scaleX;
            set
            {
                if (!MathUtl.IsSame(_scaleX, value))
                {
                    _scaleX = value;
                    OnPropertyChanged(nameof(scaleX));
                }
            }
        }
        private float? _scaleY;
        public float? scaleY
        {
            get => _scaleY;
            set
            {
                if (!MathUtl.IsSame(_scaleY, value))
                {
                    _scaleY = value;
                    OnPropertyChanged(nameof(scaleY));
                }
            }
        }
        private float? _scaleZ;
        public float? scaleZ
        {
            get => _scaleZ;
            set
            {
                if (!MathUtl.IsSame(_scaleZ, value))
                {
                    _scaleZ = value;
                    OnPropertyChanged(nameof(scaleZ));
                }
            }
        }

        public MultiSelectionTransform(MultiSelectionEntity msEntity) : base(msEntity)
        {
            Refresh();
        }
        protected override bool UpdateComponents(string PropertyName)
        {
            switch (PropertyName)
            {
                case nameof(posX):
                case nameof(posY):
                case nameof(posZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3(posX ?? c.Position.X, posY ?? c.Position.Y, posZ ?? c.Position.Z));
                    return true;
                case nameof(rotX):
                case nameof(rotY):
                case nameof(rotZ):
                    SelectedComponents.ForEach(c => c.Rotation = new Vector3(rotX ?? c.Rotation.X, rotY ?? c.Rotation.Y, rotZ ?? c.Rotation.Z));
                    return true;
                case nameof(scaleX):
                case nameof(scaleY):
                case nameof(scaleZ):
                    SelectedComponents.ForEach(c => c.Scale = new Vector3(scaleX ?? c.Scale.X, scaleY ?? c.Scale.Y, scaleZ ?? c.Scale.Z));
                    return true;
            }
            return false;
        }

        protected override bool UpdateMSComponent()
        {
            posX = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(x => x.Position.X));
            posY = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(y => y.Position.Y));
            posZ = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(z => z.Position.Z));
            rotX = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(x => x.Rotation.X));
            rotY = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(y => y.Rotation.Y));
            rotZ = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(z => z.Rotation.Z));
            scaleX = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(x => x.Scale.X));
            scaleY = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(y => y.Scale.Y));
            scaleZ = MultiSelectionEntity.GetMixedValue(SelectedComponents, new Func<Transform, float?>(z => z.Scale.Z));
            return true;
        }
    }
}
