using TinyStep.DynamicBuffers;
using TinyStep.Input;
using TinyStep.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;
using Tween = TinyStep.Tweener.Tweener;

namespace TinyStep
{
    [BurstCompile][UpdateAfter(typeof(InputHandler))]
    public class SetFallDownSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BlockMatrixData>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var setFallDownCount = GetEntityQuery(typeof(SetFallDowns)).CalculateEntityCountWithoutFiltering();
            if(setFallDownCount == 0) return;
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            
            
            if (blockMatrixData.CanIExecuteSetFallDown) 
            {
                blockMatrixData.StartFallDown(); 
                SetSingleton(blockMatrixData);
            }
            else return;
            
            Debug.Log("SetFallDown");
            
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];
            
            for (int i = 0; i < blockMatrixData.BlockMatrixDefinition.MatrixLength; i++)
            {
                if (blockDefinitionBuffers[i].Existence) continue;

                for (int j = i + blockMatrixData.BlockMatrixDefinition.MatrixScale.x; j < blockMatrixData.BlockMatrixDefinition.MatrixLength; j += blockMatrixData.BlockMatrixDefinition.MatrixScale.x)
                {
                    if (blockDefinitionBuffers[j].Existence)
                    {
                        MoveBlock(ecb,ref blockDefinitionBuffers ,blockMatrixData, j, i);
                        break;
                    }
                }
            }
            
            blockMatrixData.CompleteFallDown();
            SetSingleton(blockMatrixData);

            EntityManager.RemoveComponent<SetFallDowns>(blockMatrixEntity);
            EntityManager.AddComponent<SetCreateBlocks>(blockMatrixEntity);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        public void MoveBlock(EntityCommandBuffer ecb,ref DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers,BlockMatrixData blockMatrixData,int blockIndex, int moveToIndex)
        {
            int2 matrixScale = blockMatrixData.BlockMatrixDefinition.MatrixScale;
            Entity entity = blockDefinitionBuffers[blockIndex].Entity;
            float3 translation = GetComponent<Translation>(entity).Value;
            float3 moveToPos = BlockMatrixUtilities.GetBlockPositionLocal(
                moveToIndex,
                matrixScale,
                blockMatrixData.BlockMatrixDefinition.OneBlockScale
                );

            int moveToYPos = BlockMatrixUtilities.GetMatrixCoordFromIndex(moveToIndex, matrixScale).y;
            int yDiff = BlockMatrixUtilities.GetMatrixCoordFromIndex(blockIndex,matrixScale).y - moveToYPos;
            
            Renderer2D renderer = GetComponent<Renderer2D>(blockDefinitionBuffers[blockIndex].Entity);
            renderer.OrderInLayer = (short)(moveToYPos* 2);
            SetComponent(blockDefinitionBuffers[blockIndex].Entity,renderer);

            float duration = yDiff * 0.07f;
            Tween.Move(ecb, entity, translation, moveToPos, duration);
            blockDefinitionBuffers[moveToIndex] = blockDefinitionBuffers[blockIndex];
            blockDefinitionBuffers[blockIndex] = new BlockDefinitionBuffer( false);

        }
        
    }
}
