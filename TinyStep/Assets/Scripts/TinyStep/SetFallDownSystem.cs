using TinyStep.DynamicBuffers;
using TinyStep.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;
using Tween = TinyStep.Tweener.Tweener;

namespace TinyStep
{
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
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];
            
            var setFallDownCount = GetEntityQuery(typeof(SetFallDowns)).CalculateEntityCountWithoutFiltering();
            if (setFallDownCount > 0 && !blockMatrixData.IsWaitingForFallDown) 
            {
                blockMatrixData.StartFallDown(); 
                SetSingleton(blockMatrixData);
            }
            else return;
                
            if (blockMatrixData.MovingBlockCount > 0 || blockMatrixData.DestroyedBlockCount > 0) return;
            
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

            EntityManager.RemoveComponent<SetFallDowns>(blockMatrixEntity);
            EntityManager.AddComponent<SetCrews>(blockMatrixEntity);
            blockMatrixData.CompleteFallDown();
            SetSingleton(blockMatrixData);
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
            int yDiff = BlockMatrixUtilities.GetMatrixCoordFromIndex(blockIndex,matrixScale).y -
                        BlockMatrixUtilities.GetMatrixCoordFromIndex(moveToIndex,matrixScale).y;
            float duration = yDiff * 0.2f;
            Tween.Move(ecb, entity, translation, moveToPos, duration);
            blockDefinitionBuffers[moveToIndex] = blockDefinitionBuffers[blockIndex];
            blockDefinitionBuffers[blockIndex] = new BlockDefinitionBuffer( false);

        }
        
    }
}
