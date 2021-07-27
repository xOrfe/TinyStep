using TinyStep.Commands;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TinyStep
{
    public class MoveSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.
                WithAll<Block>().
                ForEach((int entityInQueryIndex, Entity entity, ref Translation translation) =>
                {
                    MoveCommand moveCommand = new MoveCommand()
                    {
                        moveTo = new float2(-3, -3),
                        speed = 1
                    };
                    ecb.AddComponent(entityInQueryIndex,entity,moveCommand);
                }).Schedule();
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        protected override void OnUpdate()
        {
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.
                ForEach((int entityInQueryIndex,Entity entity,ref Translation translation,in MoveCommand moveCommand) =>
                {
                    float2 thisPos = new float2(translation.Value.x, translation.Value.y);
                    if (math.distance(thisPos,moveCommand.moveTo) < 0.05f)
                    {
                        translation.Value = new float3(moveCommand.moveTo,translation.Value.z);
                        ecb.RemoveComponent<MoveCommand>(entityInQueryIndex,entity);
                        return;
                    }

                    float3 dir = new float3(math.normalize(moveCommand.moveTo - thisPos) * moveCommand.speed,translation.Value.z);
                    translation.Value += dir;
                }).Schedule();
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

        }
    }
}
