using Unity.Entities;

namespace TinyStep
{
    [GenerateAuthoringComponent]
    public struct BlockMatrixData : IComponentData
    {
        public int BlockCount;
        public int MovingBlockCount; 
    }
}
