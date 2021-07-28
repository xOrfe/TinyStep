using TinyStep.Tweener;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using AOT;
using TinyStep.Input;
using TinyStep.Utils;
using UnityEngine;
using Sprite = Unity.Tiny.Sprite;
using SpriteRenderer = Unity.Tiny.SpriteRenderer;
using Tween = TinyStep.Tweener.Tweener;

namespace TinyStep
{
    public class BlockMatrixSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private EntityArchetype _blockAcheType;
        protected override void OnCreate()
        {

            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<InputData>();
            RequireSingletonForUpdate<BlockSpawnComponent>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnStartRunning()
        {

            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var prefabContainer = GetSingleton<BlockSpawnComponent>();
            
            _blockAcheType = EntityManager.CreateArchetype(typeof(Translation),typeof(Rotation),typeof(Block));
            
            int matrixLength = blockMatrixData.BlockCount;

            for (int i = 0; i < matrixLength; i++)
            {
                Entity spawnedEntity = EntityManager.CreateEntity(_blockAcheType);
                Translation trns = new Translation()
                {
                    Value = new float3(BlockMatrixUtilities.GetBlockPositionLocal(i,blockMatrixData.BlockMatrixDefinition.MatrixScale,blockMatrixData.BlockMatrixDefinition.OneBlockScale))
                };
                EntityManager.SetComponentData(spawnedEntity,trns);
            }
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var blockMatrixData = GetSingleton<BlockMatrixData>();

            Entities.
                WithAll<MoveOrderOnComplete>().
                ForEach((Entity entity) => {
                    blockMatrixData.ABlockStopMoving();
                    
                    World.EntityManager.RemoveComponent<MoveOrderOnComplete>(entity);
                }).WithoutBurst().WithStructuralChanges().Run();
            
            
            
            var activeInput = GetSingleton<InputData>();
            
            if (!activeInput.IsTouchExecuted)
            {
                Entities.
                    WithAll<Block>().
                    ForEach((Entity entity,in Translation translation) => {
                        Tween.Move(ecb, entity, translation.Value, new float3(activeInput.Pos.x,activeInput.Pos.y,0), 1.0f);
                        blockMatrixData.ABlockStartMoving();
                    }).WithoutBurst().Run();
                
                
                activeInput.IsTouchExecuted = true;
                SetSingleton(activeInput);
            }
            
            
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        [BurstCompile]
        public void CreateBlock()
        {
            
        }
    }
}
