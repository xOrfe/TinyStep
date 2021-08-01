using TinyStep.DynamicBuffers;
using TinyStep.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Tiny;

namespace TinyStep.Input
{
    [BurstCompile]
    public class InputHandler : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<InputData>();
        }

        protected override void OnUpdate()
        {
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            var activeInput = GetSingleton<InputData>();

            if (!blockMatrixData.CanIExecuteInputHandle || activeInput.IsTouchExecuted) return;
            
            int touchIndex = BlockMatrixUtilities.GetIndexFromWorld(activeInput.Pos,
                blockMatrixData.BlockMatrixDefinition.OneBlockScale, blockMatrixData.BlockMatrixDefinition.MatrixScale);
            if (touchIndex < 0 || touchIndex > blockMatrixData.BlockMatrixDefinition.MatrixLength - 1) return;
            Debug.Log("InputHandler");

            blockMatrixData.StartInputHandle();
            
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers =
                GetBufferFromEntity<BlockDefinitionBuffer>(false)[blockMatrixEntity];
            
            if (blockDefinitionBuffers[touchIndex].Existence)
            {
                int crewIndex = blockDefinitionBuffers[touchIndex].CrewIndex;
                if (crewIndex != -1)
                {
                    for (int i = 0; i < blockDefinitionBuffers.Length; i++)
                    {
                        if (blockDefinitionBuffers[i].CrewIndex == crewIndex)
                        {
                            EntityManager.DestroyEntity(blockDefinitionBuffers[i].Entity);
                            blockDefinitionBuffers =
                                GetBufferFromEntity<BlockDefinitionBuffer>(false)[blockMatrixEntity];
                            blockDefinitionBuffers[i] = new BlockDefinitionBuffer(false);
                            blockMatrixData.BlockDestroyed();
                        }
                    }
                    
                    EntityManager.AddComponent<SetFallDowns>(blockMatrixEntity);
                }
                activeInput.IsTouchExecuted = true;
                SetSingleton(activeInput);
            }
            
            blockMatrixData.CompleteInputHandle();
            SetSingleton(blockMatrixData);
        }
    }
}