
using System;
using Unity.Collections;
using Unity.Entities;
namespace TinyStep
{
    public struct BlockSpawner : IComponentData
    {
        public Entity RedBlockEntity;
        public Entity GreenBlockEntity;
        public Entity BlueBlockEntity;
        public Entity YellowBlockEntity;
    }
    
    [Serializable] public struct MoveOrderOnStart : IComponentData {}
    [Serializable] public struct MoveOrderOnComplete : IComponentData {}
    [Serializable] public struct SetCrews : IComponentData {}
    [Serializable] public struct SetFallDowns : IComponentData {}
}