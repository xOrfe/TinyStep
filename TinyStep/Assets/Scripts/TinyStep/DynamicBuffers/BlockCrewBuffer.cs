using Unity.Collections;
using Unity.Entities;

namespace TinyStep.DynamicBuffers
{
    [GenerateAuthoringComponent][InternalBufferCapacity(40)]//Max crew count is 40 in 9x9 matrix
    public struct BlockCrewBuffer : IBufferElementData
    {
        public FixedListInt64 Crew;
    }
}