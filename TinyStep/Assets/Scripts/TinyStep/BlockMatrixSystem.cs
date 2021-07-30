using System;
using TinyStep.DynamicBuffers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using TinyStep.Input;
using TinyStep.Utils;
using Unity.Collections;
using Unity.Tiny;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;
using Tween = TinyStep.Tweener.Tweener;

namespace TinyStep
{
    public class BlockMatrixSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private EntityArchetype _blockAcheType;
        private Random _random;
        private Entity _blockMatrixEntity;
        private NativeArray<Entity> _blockPrefabs;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BlockSpawner>();
            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<InputData>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _random = new Random(54455474);
        }
        
        protected override void OnStartRunning()
        {
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            var blockSpawner = GetSingleton<BlockSpawner>();
            
            _blockMatrixEntity = GetSingletonEntity<BlockSpawner>();

            //i made this with brute force bcz there is no other way includes unsafe options to store array in IComponentData
            _blockPrefabs = new NativeArray<Entity>(4,Allocator.Persistent);
            _blockPrefabs[0] = blockSpawner.RedBlockEntity;
            _blockPrefabs[1] = blockSpawner.GreenBlockEntity;
            _blockPrefabs[2] = blockSpawner.BlueBlockEntity;
            _blockPrefabs[3] = blockSpawner.YellowBlockEntity;

            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            int matrixLength = blockMatrixData.BlockMatrixDefinition.MatrixLength;
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];

            for (int i = 0; i < matrixLength; i++)
            {
                blockDefinitionBuffers.Add(new BlockDefinitionBuffer());
            }
            for (int i = 0; i < matrixLength; i++)
            {
                CreateBlock(ecb,blockMatrixEntity,i,true);
            }
            ecb.AddComponent<SetCrews>(blockMatrixEntity);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        [BurstCompile]
        public void CreateBlock(EntityCommandBuffer ecb,Entity blockMatrixEntity,int matrixIndex,bool createOnPlace)
        {
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockSpawner = GetSingleton<BlockSpawner>();
            var blockColor = _random.NextInt(4);
            Entity spawnedEntity = EntityManager.Instantiate(_blockPrefabs[blockColor]);
            Translation trns = new Translation()
            {
                Value = new float3(BlockMatrixUtilities.GetBlockPositionLocal(matrixIndex,blockMatrixData.BlockMatrixDefinition.MatrixScale,blockMatrixData.BlockMatrixDefinition.OneBlockScale))
            };
            EntityManager.SetComponentData(spawnedEntity,trns);
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];
            blockDefinitionBuffers.RemoveAt(matrixIndex);
            blockDefinitionBuffers.Insert(matrixIndex,new BlockDefinitionBuffer(spawnedEntity,matrixIndex,blockColor));
            
            SetSingleton(blockMatrixData);
        }
        
        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            var activeInput = GetSingleton<InputData>();
            
            DynamicBuffer<BlockDefinitionBuffer> blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>()[blockMatrixEntity];
            var blockCrewBuffers = GetBufferFromEntity<BlockCrewBuffer>()[blockMatrixEntity];

            if (!activeInput.IsTouchExecuted)
            {
                int touchIndex = BlockMatrixUtilities.GetIndexFromWorld(activeInput.Pos,blockMatrixData.BlockMatrixDefinition.OneBlockScale,blockMatrixData.BlockMatrixDefinition.MatrixScale);
                if(touchIndex < 0 || touchIndex > blockMatrixData.BlockMatrixDefinition.MatrixLength - 1) return;
                if(blockDefinitionBuffers[touchIndex].Existence)
                {
                    int crewIndex = blockDefinitionBuffers[touchIndex].CrewIndex;
                    if (crewIndex != -1)
                    {
                        foreach (var i in blockCrewBuffers[crewIndex].Crew)
                        {
                            ecb.DestroyEntity(blockDefinitionBuffers[i].Entity);
                            blockDefinitionBuffers[i] = new BlockDefinitionBuffer( false);
                        }
                        EntityManager.AddComponent<SetFallDowns>(blockMatrixEntity);
                    }
                }
                activeInput.IsTouchExecuted = true;
                SetSingleton(activeInput);
            }
            
            SetSingleton(blockMatrixData);
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        
        

        protected override void OnStopRunning()
        {
            _blockPrefabs.Dispose();
        }
    }
}
