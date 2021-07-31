using TinyStep.DynamicBuffers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;

namespace TinyStep
{
    public class SetCrewsJobSystem : JobComponentSystem
    {
        public struct SetCrewsJob : IJob
        {
            public BlockMatrixData BlockMatrixData;
            public NativeArray<BlockDefinitionBuffer> BlockDefinitionBuffers;
            
            public void Execute()
            {
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
                        MatchBlocks(ref BlockDefinitionBuffers,ref crewCount, i, rightBlockIndex);
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
                        MatchBlocks(ref BlockDefinitionBuffers,ref crewCount, i, upBlockIndex);
                    }
                }
                
            }
        }

        public static void MatchBlocks(ref NativeArray<BlockDefinitionBuffer> blockDefinitions,ref int crewCount,int b1,int b2)
        {
            if (blockDefinitions[b1].CrewIndex == -1 && blockDefinitions[b2].CrewIndex == -1)
            {
                BlockDefinitionBuffer b1Def = blockDefinitions[b1];
                b1Def.CrewIndex = crewCount;
                blockDefinitions[b1] = b1Def;
                
                BlockDefinitionBuffer b2Def = blockDefinitions[b2];
                b2Def.CrewIndex = crewCount;
                blockDefinitions[b2] = b2Def;
                
                crewCount++;
                return;
            }
            if (blockDefinitions[b1].CrewIndex != -1 && blockDefinitions[b2].CrewIndex == -1)
            {
                BlockDefinitionBuffer b2Def = blockDefinitions[b2];
                b2Def.CrewIndex = blockDefinitions[b1].CrewIndex;
                blockDefinitions[b2] = b2Def;
                return;
            }
            if (blockDefinitions[b1].CrewIndex == -1 && blockDefinitions[b2].CrewIndex != -1)
            {
                BlockDefinitionBuffer b1Def = blockDefinitions[b1];
                b1Def.CrewIndex = blockDefinitions[b2].CrewIndex;
                blockDefinitions[b1] = b1Def;
            }
            else
            {
                int b2TemporaryCrewIndex = blockDefinitions[b2].CrewIndex;

                for (int i = 0; i < blockDefinitions.Length; i++)
                {
                    if (blockDefinitions[i].CrewIndex == b2TemporaryCrewIndex)
                    {
                        BlockDefinitionBuffer b2MemberDef = blockDefinitions[i];
                        b2MemberDef.CrewIndex = blockDefinitions[b1].CrewIndex;
                        blockDefinitions[i] = b2MemberDef;
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var setCrewCount = GetEntityQuery(typeof(SetCrews)).CalculateEntityCountWithoutFiltering();
            if (setCrewCount == 0) return inputDeps;
            
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
            
            SetCrewsJob setCrewsJob = new SetCrewsJob
            {
                BlockMatrixData = blockMatrixData,
                BlockDefinitionBuffers = blockDefinitionBuffers.ToNativeArray(Allocator.TempJob)
            };
            
            var jobHandle = setCrewsJob.Schedule(inputDeps);
            
            jobHandle.Complete();
            
            blockDefinitionBuffers.Clear();
            blockDefinitionBuffers.AddRange(setCrewsJob.BlockDefinitionBuffers);
            setCrewsJob.BlockDefinitionBuffers.Dispose();
            

            EntityManager.RemoveComponent<SetCrews>(blockMatrixEntity);
            blockMatrixData.CompleteSetCrews();
            SetSingleton(blockMatrixData);
            
            return jobHandle;
        }
    }
}
