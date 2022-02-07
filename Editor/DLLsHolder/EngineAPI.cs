using Editor.Components;
using Editor.EngineAPIStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.DLLsHolder
{
  
    static class EngineAPI
    {
        private const string _dllName = "Enginedll.dll";

        [DllImport(_dllName)]
        private static extern int CreateGameEntity(GameEntityDescriptor desc);
        public static int CreateGameEntity(GameEntity entity)
        {
            GameEntityDescriptor desc = new GameEntityDescriptor();

            //transform
            {
                var c = entity.GetComponent<Transform>();
                desc.transformComponent.position = c.Position;
                desc.transformComponent.rotation = c.Rotation;
                desc.transformComponent.scale = c.Scale;
            }
            return CreateGameEntity(desc);
        }

        [DllImport(_dllName)]
        private static extern void RemoveGameEntity(int id);
        public static void RemoveGameEntity(GameEntity entity)
        {
            RemoveGameEntity(entity.EntityID);
        }
    }
}

namespace Editor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = new Vector3(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent transformComponent = new TransformComponent();
    }
}

