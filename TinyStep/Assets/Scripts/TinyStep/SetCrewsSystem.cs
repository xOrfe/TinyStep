using TinyStep;
using TinyStep.DynamicBuffers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(SetFallDownSystem))]
public class SetCrewsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Forvarded to Job System, check "SetCrewsJobSystem"
        // This folder still here bcz this is usable for debugging things 
        
        return;
        var setCrewCount = GetEntityQuery(typeof(SetCrews)).CalculateEntityCountWithoutFiltering();
        if (setCrewCount == 0) return;
            
        var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            
        var blockMatrixData = GetSingleton<BlockMatrixData>();
            
        var blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];
            
        blockMatrixData.StartSetCrews();
        
        for (int i = 0; i < blockMatrixData.BlockMatrixDefinition.MatrixLength; i++)
        {
            BlockDefinitionBuffer blockDefinitionBuffer = blockDefinitionBuffers[i];
            blockDefinitionBuffer.CrewIndex = -1;
            blockDefinitionBuffers[i] = blockDefinitionBuffer;
        }
        
        BlockMatrixData BlockMatrixData; 
        NativeArray<BlockDefinitionBuffer> BlockDefinitionBuffers;

        BlockMatrixData = blockMatrixData;
        BlockDefinitionBuffers = blockDefinitionBuffers.ToNativeArray(Allocator.Temp);
        
        int crewCount = 0;
        int matrixLength = BlockMatrixData.BlockMatrixDefinition.MatrixLength;
        int2 matrixScale = BlockMatrixData.BlockMatrixDefinition.MatrixScale;
                
        for (int i = 0; i < matrixLength; i++)
        {
            int rightBlockIndex = i + 1;

            if (!BlockDefinitionBuffers[i].Existence || (i + 1) % matrixScale.x == 0) continue;

            if (BlockDefinitionBuffers[rightBlockIndex].Existence 
                && BlockDefinitionBuffers[i].BlockType == BlockDefinitionBuffers[rightBlockIndex].BlockType
                && BlockDefinitionBuffers[i].BlockColor == BlockDefinitionBuffers[rightBlockIndex].BlockColor )
            {
                SetCrewsJobSystem.MatchBlocks(ref BlockDefinitionBuffers,ref crewCount, i, rightBlockIndex);
            }
        }
                
        for (int i = 0; i < matrixLength - matrixScale.x; i++)
        {
            int upBlockIndex = i + matrixScale.x;

            if (!BlockDefinitionBuffers[i].Existence) continue;

            if (BlockDefinitionBuffers[upBlockIndex].Existence &&
                BlockDefinitionBuffers[i].BlockType == BlockDefinitionBuffers[upBlockIndex].BlockType
                && BlockDefinitionBuffers[i].BlockColor == BlockDefinitionBuffers[upBlockIndex].BlockColor)
            {
                SetCrewsJobSystem.MatchBlocks(ref BlockDefinitionBuffers,ref crewCount, i, upBlockIndex);
            }
        }
        
        blockDefinitionBuffers.Clear();
        blockDefinitionBuffers.AddRange(BlockDefinitionBuffers);
        BlockDefinitionBuffers.Dispose();
        
        EntityManager.RemoveComponent<SetCrews>(blockMatrixEntity);
        
        blockMatrixData.CompleteSetCrews();
        SetSingleton(blockMatrixData);
    }
}

