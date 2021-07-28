using Unity.Entities;

namespace TinyStep.Buffers
{
    [InternalBufferCapacity(81)][GenerateAuthoringComponent]
    public struct BlockEntityBuffer : IBufferElementData
    {
        //Stores entity ındex, points block bool buffers entity index.
        public int BlockBool;
    }
}
