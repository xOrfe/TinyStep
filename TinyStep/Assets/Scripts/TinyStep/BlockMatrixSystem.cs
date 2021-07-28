using TinyStep.Tweener;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using TinyStep.Input;
using TinyStep.Utils;
using Unity.Collections;
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
        private Random _Random;
        
        private NativeArray<BlockSprite> _blockSprites;
        private SpriteRenderer _spriteRenderer;
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BlockSpawner>();
            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<InputData>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _Random = new Random(54455474);

        }
        
        protected override void OnStartRunning()
        {

            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockSpawner = GetSingleton<BlockSpawner>();
            var blockSpawnerEntity = GetSingletonEntity<BlockSpawner>();
            
            _blockSprites = EntityManager.GetBuffer<BlockSprite>(blockSpawnerEntity).AsNativeArray();
            _spriteRenderer = EntityManager.GetComponentData<SpriteRenderer>(blockSpawner.Prefab);


            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            int matrixLength = blockMatrixData.BlockCount;
            for (int i = 0; i < matrixLength; i++)
            {
                CreateBlock(i,ecb);
            }
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
            

        }
        [BurstCompile]
        public void CreateBlock(int matrixIndex,EntityCommandBuffer ecb)
        {
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockSpawner = GetSingleton<BlockSpawner>();
            

            Entity spawnedEntity = EntityManager.Instantiate(blockSpawner.Prefab);
            _spriteRenderer.Sprite = _blockSprites[_Random.NextInt(3)].Sprite; 
            EntityManager.SetComponentData(spawnedEntity,_spriteRenderer);
            Translation trns = new Translation()
            {
                Value = new float3(BlockMatrixUtilities.GetBlockPositionLocal(matrixIndex,blockMatrixData.BlockMatrixDefinition.MatrixScale,blockMatrixData.BlockMatrixDefinition.OneBlockScale))
            };
            EntityManager.SetComponentData(spawnedEntity,trns);
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

        
    }
}
