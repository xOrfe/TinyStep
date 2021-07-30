using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TinyStep.Tweener
{
    public class Ease : SystemBase
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
            var ecbParallel = ecb.AsParallelWriter();

            var blockMatrixData = GetSingleton<BlockMatrixData>();
            
            EntityQuery moveOrderOnStartQuery = GetEntityQuery(typeof(MoveOrderOnStart));
            EntityQuery moveOrderOnCompleteQuery = GetEntityQuery(typeof(MoveOrderOnComplete));
            
            blockMatrixData.BlockStartMoving(moveOrderOnStartQuery.CalculateEntityCount());
            blockMatrixData.BlockStopMoving(moveOrderOnCompleteQuery.CalculateEntityCount());
            
            ecb.RemoveComponentForEntityQuery<MoveOrderOnStart>(moveOrderOnStartQuery);
            ecb.RemoveComponentForEntityQuery<MoveOrderOnComplete>(moveOrderOnCompleteQuery);
            
            float deltaTime = Time.DeltaTime;
            Entities
                .ForEach((Entity entity,ref Translation translation,ref MoveOrder moveOrder) =>
                {
                    float3 startPos = moveOrder.start;
                    float3 endPos = moveOrder.end;
                    moveOrder.deltaTime += deltaTime;
                    if (moveOrder.deltaTime > moveOrder.duration)
                    {
                        translation.Value = endPos;
                        ecbParallel.RemoveComponent<MoveOrder>(0,entity);
                        ecbParallel.AddComponent<MoveOrderOnComplete>(1,entity);
                        return;
                    }
                    var percentage = moveOrder.deltaTime / moveOrder.duration;
                    var ease = EaseJobs.Linear(percentage);
                    float3 trns = translation.Value;
                    ActionJobs.Float3To(ref trns,ref startPos,ref endPos, ease);
                    translation.Value = trns;
                }).Schedule();
            
            SetSingleton(blockMatrixData);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

        }
    }
}
