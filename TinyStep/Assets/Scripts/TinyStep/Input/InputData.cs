using Unity.Entities;
using Unity.Mathematics;

namespace TinyStep.Input
{
    [GenerateAuthoringComponent]
    public struct InputData : IComponentData
    {
        public float2 Pos;
        public bool IsTouchExecuted;
    }
}
