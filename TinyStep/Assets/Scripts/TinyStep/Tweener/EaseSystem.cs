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
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities
                .ForEach((Entity entity,ref Translation translation,in MoveOrder moveOrder) =>
                {
                    float currentTime = Tweener.GetTime();
                    float3 startPos = moveOrder.start;
                    float3 endPos = moveOrder.end;
                    var orderEndTime = moveOrder.startTime + moveOrder.duration;
                    if (currentTime > orderEndTime)
                    {
                        ecb.RemoveComponent<MoveOrder>(0,entity);
                        ecb.AddComponent<MoveOrderOnComplete>(1,entity);
                        return;
                    }
                    var percentage = (currentTime - moveOrder.startTime) / moveOrder.duration;
                    var ease = EaseJobs.Linear(percentage);
                    float3 trns = translation.Value;
                    ActionJobs.Float3To(ref trns,ref startPos,ref endPos, ease);
                    translation.Value = trns;
                }).Schedule();
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

        }
    }
}
