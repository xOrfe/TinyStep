using Unity.Entities;

namespace TinyStep.Buffers
{
    [InternalBufferCapacity(81)][GenerateAuthoringComponent]
    public struct BlockBoolBuffer : IBufferElementData
    {
        //Stores blocks existing, points blockEntityBuffers entity existing.
        public bool BlockBool;
    }
}
