using System;
using Unity.Entities;
using Unity.Mathematics;

namespace TinyStep.Commands
{
    [Serializable]
    public struct MoveCommand : IComponentData
    {
        public float2 moveTo;
        public float speed;
    }
}
