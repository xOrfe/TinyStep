using System.Linq;
using TinyStep.DynamicBuffers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace TinyStep
{
    public class SetCrewsJobSystem : JobComponentSystem
    {
        public struct SetCrewsJob : IJob
        {
            public BlockMatrixData BlockMatrixData;
            public NativeArray<BlockDefinitionBuffer> BlockDefinitionBuffers;
            public NativeArray<BlockCrewBuffer> BlockCrewBuffers;
            
            public void Execute()
            {
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
                        MatchBlocks(ref BlockDefinitionBuffers,ref BlockCrewBuffers, i, rightBlockIndex);
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
                        MatchBlocks(ref BlockDefinitionBuffers,ref BlockCrewBuffers, i, upBlockIndex);
                    }
                }
                
            }
        }

        public static void MatchBlocks(ref NativeArray<BlockDefinitionBuffer> blockDefinitions,ref NativeArray<BlockCrewBuffer> blockCrews,int b1,int b2)
        {
            if (blockDefinitions[b1].CrewIndex == -1 && blockDefinitions[b2].CrewIndex == -1)
            {
                BlockCrewBuffer crew = new BlockCrewBuffer(){};
                crew.Crew = new FixedListInt64();
                crew.Crew.Add(b1);
                crew.Crew.Add(b2);

                NativeList<BlockCrewBuffer> tmpBuffers = new NativeList<BlockCrewBuffer>(Allocator.Temp);
                tmpBuffers.AddRange(blockCrews);
                tmpBuffers.Add(crew);

                blockCrews = tmpBuffers;
                
                BlockDefinitionBuffer b1Def = blockDefinitions[b1];
                b1Def.CrewIndex = blockCrews.Length - 1;
                blockDefinitions[b1] = b1Def;
                
                BlockDefinitionBuffer b2Def = blockDefinitions[b2];
                b2Def.CrewIndex = blockCrews.Length - 1;
                blockDefinitions[b2] = b2Def;
            }
            if (blockDefinitions[b1].CrewIndex != -1 && blockDefinitions[b2].CrewIndex == -1)
            {
                blockCrews[blockDefinitions[b1].CrewIndex].Crew.Add(b2);
                
                BlockDefinitionBuffer b2Def = blockDefinitions[b2];
                b2Def.CrewIndex = blockDefinitions[b1].CrewIndex;
                blockDefinitions[b2] = b2Def;
            }
            if (blockDefinitions[b1].CrewIndex == -1 && blockDefinitions[b2].CrewIndex != -1)
            {
                blockCrews[blockDefinitions[b2].CrewIndex].Crew.Add(b1);
                
                BlockDefinitionBuffer b1Def = blockDefinitions[b1];
                b1Def.CrewIndex = blockDefinitions[b2].CrewIndex;
                blockDefinitions[b1] = b1Def;
            }
            else
            {
                int b2TemporaryCrewIndex = blockDefinitions[b2].CrewIndex;
                foreach (var b2Member in blockCrews[blockDefinitions[b2].CrewIndex].Crew)
                {
                    blockCrews[blockDefinitions[b1].CrewIndex].Crew.Add(b2Member);
                    
                    BlockDefinitionBuffer b2MemberDef = blockDefinitions[b2Member];
                    b2MemberDef.CrewIndex = blockDefinitions[b1].CrewIndex;
                    blockDefinitions[b1] = b2MemberDef;
                }
                blockCrews[b2TemporaryCrewIndex].Crew.Clear();
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var setCrewCount = GetEntityQuery(typeof(SetCrews)).CalculateEntityCountWithoutFiltering();
            if (setCrewCount == 0) return inputDeps;
            
            var blockMatrixEntity = GetSingletonEntity<BlockMatrixData>();
            
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            var blockDefinitionBuffers = GetBufferFromEntity<BlockDefinitionBuffer>(true)[blockMatrixEntity];
            var blockCrewBuffers = GetBufferFromEntity<BlockCrewBuffer>()[blockMatrixEntity];
            
            blockMatrixData.StartSetCrews();
            
            
            
            SetCrewsJob setCrewsJob = new SetCrewsJob
            {
                BlockMatrixData = blockMatrixData,
                BlockDefinitionBuffers = blockDefinitionBuffers.ToNativeArray(Allocator.TempJob),
                BlockCrewBuffers = blockCrewBuffers.ToNativeArray(Allocator.TempJob)
            };
            
            var jobHandle = setCrewsJob.Schedule(inputDeps);
            
            jobHandle.Complete();
            
            blockDefinitionBuffers.Clear();
            blockDefinitionBuffers.AddRange(setCrewsJob.BlockDefinitionBuffers);
            
            blockCrewBuffers.Clear();
            blockCrewBuffers.AddRange(setCrewsJob.BlockCrewBuffers);
            
            EntityManager.RemoveComponent<SetCrews>(blockMatrixEntity);
            blockMatrixData.CompleteSetCrews();
            SetSingleton(blockMatrixData);
            
            return jobHandle;
        }
    }
}
