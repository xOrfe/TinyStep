using TinyStep.DynamicBuffers;
using TinyStep.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;
using Tween = TinyStep.Tweener.Tweener;
namespace TinyStep
{
    [BurstCompile][UpdateAfter(typeof(SetCrewsSystem))]
    public class BlockSpawnSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private Random _random;
        private NativeArray<Entity> _blockPrefabs;
        
        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<BlockSpawner>();
        }
        

        protected override void OnStartRunning()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            var blockSpawner = GetSingleton<BlockSpawner>();
            
            _random = new Random(54455474);

            //
            //i made this with brute force bcz there is no other way including unsafe options to store prefab array in IComponentData which sync go world and entity world
            //                                                                              
            _blockPrefabs = new NativeArray<Entity>(4, Allocator.Persistent);
            _blockPrefabs[0] = blockSpawner.RedBlockEntity;
            _blockPrefabs[1] = blockSpawner.GreenBlockEntity;
            _blockPrefabs[2] = blockSpawner.BlueBlockEntity;
            _blockPrefabs[3] = blockSpawner.YellowBlockEntity;
            
            

            int matrixLength = blockMatrixData.BlockMatrixDefinition.MatrixLength;
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers =
                GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];

            for (int i = 0; i < matrixLength; i++)
            {
                blockDefinitionBuffers.Add(new BlockDefinitionBuffer());
            }

            for (int i = 0; i < matrixLength; i++)
            {
                CreateBlock(ecb, blockMatrixEntity, i,0, true);
            }

            ecb.AddComponent<SetCrews>(blockMatrixEntity);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        protected override void OnUpdate()
        {
            var setBlockSpawnCount = GetEntityQuery(typeof(SetCreateBlocks)).CalculateEntityCountWithoutFiltering();
            if(setBlockSpawnCount == 0) return;
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();


            if (blockMatrixData.CanIExecuteBlockSpawn)
            {
                blockMatrixData.StartBlocksCreate();
                SetSingleton(blockMatrixData);
            }
            else return;
            
            Debug.Log("Spawn");

            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers =
                GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];

            bool ySetted = false;
            int yFirst = 0;
            for (int i = 0; i < blockMatrixData.BlockMatrixDefinition.MatrixLength; i++)
            {
                blockDefinitionBuffers =
                    GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];
                if (!blockDefinitionBuffers[i].Existence)
                {
                    if (!ySetted)
                    {
                        int2 pos = BlockMatrixUtilities.GetMatrixCoordFromIndex(i,blockMatrixData.BlockMatrixDefinition.MatrixScale);
                        yFirst = pos.y;
                        ySetted = true;
                    }
                    blockMatrixData.BlockSpawned();
                    CreateBlock(ecb, blockMatrixEntity, i ,yFirst, false);
                }
            }
            
            EntityManager.RemoveComponent<SetCreateBlocks>(blockMatrixEntity);
            EntityManager.AddComponent<SetCrews>(blockMatrixEntity);
            blockMatrixData.CompleteBlocksCreate();
            SetSingleton(blockMatrixData);
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        [BurstCompile]
        public void CreateBlock(EntityCommandBuffer ecb, Entity blockMatrixEntity, int matrixIndex,int yFirst, bool createOnPlace)
        {
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockSpawner = GetSingleton<BlockSpawner>();
            var blockColor = _random.NextInt(4);

            Entity spawnedEntity = EntityManager.Instantiate(_blockPrefabs[blockColor]);

            int2 matrixScale = blockMatrixData.BlockMatrixDefinition.MatrixScale;
            int2 matrixPos = BlockMatrixUtilities.GetMatrixCoordFromIndex(matrixIndex,matrixScale);

            Translation trns;
            Translation trnsPlace = new Translation()
            {
                Value = new float3(BlockMatrixUtilities.GetBlockPositionLocal(matrixIndex,
                    blockMatrixData.BlockMatrixDefinition.MatrixScale,
                    blockMatrixData.BlockMatrixDefinition.OneBlockScale))
            };

            if (createOnPlace)
                trns = trnsPlace;
            else
            {
                Translation trnsTop;
                int yDiff = (int)math.distance(matrixPos.y,yFirst);
                matrixPos = new int2(matrixPos.x,blockMatrixData.BlockMatrixDefinition.MatrixScale.y + 4 + yDiff);
                
                trnsTop = new Translation()
                {
                    Value = new float3(BlockMatrixUtilities.GetBlockPositionLocal(matrixPos,
                        matrixScale,
                        blockMatrixData.BlockMatrixDefinition.OneBlockScale)) + 
                            new float3(0,yDiff * 0.15f,0)
                };
                trns = trnsTop;
                
                float duration = (trnsTop.Value.y - trnsPlace.Value.y) * 0.07f;
                Tween.Move(ecb, spawnedEntity, trnsTop.Value, trnsPlace.Value, duration);
            }
            
            EntityManager.SetComponentData(spawnedEntity, trns);
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers =
                GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];

            blockDefinitionBuffers.RemoveAt(matrixIndex);
            blockDefinitionBuffers.Insert(matrixIndex,
                new BlockDefinitionBuffer(spawnedEntity, matrixIndex, blockColor));
            
            Renderer2D renderer = GetComponent<Renderer2D>(spawnedEntity);
            renderer.OrderInLayer = (short)(matrixPos.y * 2);
            SetComponent(spawnedEntity,renderer);
            SetSingleton(blockMatrixData);
        }

        protected override void OnStopRunning()
        {
            _blockPrefabs.Dispose();
        }
    }
}