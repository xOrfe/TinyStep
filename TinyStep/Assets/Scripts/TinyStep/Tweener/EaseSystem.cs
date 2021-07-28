using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TinyStep.Tweener
{
    public class Ease : SystemBase
    {
        protected override void OnUpdate()
        {
        
        
            Entities
                .ForEach((ref Translation translation,in MoveOrder moveOrder) =>
                {
                    float currentTime = Tweener.GetTime();
                    float3 startPos = moveOrder.start;
                    float3 endPos = moveOrder.end;
                    var orderEndTime = moveOrder.startTime + moveOrder.duration;
                    if (currentTime > orderEndTime)
                    {
                        
                        return;
                    }
                    var percentage = (currentTime - moveOrder.startTime) / moveOrder.duration;
                    var ease = moveOrder.MyEaseMethod.Invoke(percentage);
                    float3 trns = translation.Value;
                    moveOrder.MyActionMethod.Invoke(ref trns,ref startPos,ref endPos, ease);
                    translation.Value = trns;
                }).Schedule();
        }
    }
}
