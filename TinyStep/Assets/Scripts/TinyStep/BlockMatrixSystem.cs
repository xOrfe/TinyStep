using TinyStep.Tweener;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using TinyStep.Input;
using TinyStep.Utils;
using Unity.Tiny;
using Debug = Unity.Tiny.Debug;
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
            RequireSingletonForUpdate<BlockSpawner>();
            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<InputData>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnStartRunning()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            var blockSpawner = GetSingleton<BlockSpawner>();
            var blockSpawnerEntity = GetSingletonEntity<BlockSpawner>();
            var blockSprites = EntityManager.GetBuffer<BlockSprite>(blockSpawnerEntity).ElementAt(0);
            
            var blockPrefabSpriteRenderer = EntityManager.GetComponentData<SpriteRenderer>(blockSpawner.Prefab);
            
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var sceneTag = EntityManager.GetSharedComponentData<SceneTag>(blockSpawnerEntity);
            var sceneSection = EntityManager.GetSharedComponentData<SceneSection>(blockSpawnerEntity);
            var localToWorld = EntityManager.GetComponentData<LocalToWorld>(blockSpawnerEntity);
            
            _blockAcheType = EntityManager.CreateArchetype(
                typeof(SceneTag),
                typeof(SceneSection),
                typeof(SpriteRenderer),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Rotation),
                typeof(Block)
                );
            
            int matrixLength = blockMatrixData.BlockCount;

            for (int i = 0; i < matrixLength; i++)
            {
                Entity spawnedEntity = EntityManager.Instantiate(blockSpawner.Prefab);
                blockPrefabSpriteRenderer.Sprite = blockSprites.Sprite; 
                EntityManager.SetComponentData(spawnedEntity,blockPrefabSpriteRenderer);
                Translation trns = new Translation()
                {
                    Value = new float3(BlockMatrixUtilities.GetBlockPositionLocal(i,blockMatrixData.BlockMatrixDefinition.MatrixScale,blockMatrixData.BlockMatrixDefinition.OneBlockScale))
                };
                EntityManager.SetComponentData(spawnedEntity,trns);
                
                Debug.Log(EntityManager.GetComponentData<Translation>(spawnedEntity).Value);
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
