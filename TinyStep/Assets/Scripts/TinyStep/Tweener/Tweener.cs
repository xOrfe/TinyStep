using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TinyStep.Tweener
{
    public static class Tweener
    {
        public static void Move(
            in EntityCommandBuffer.ParallelWriter parallelWriter, 
            in int sortKey, 
            in Entity entity, 
            in float3 start, 
            in float3 end, 
            in float duration,
            in FunctionPointer<BlockMatrixSystem.OnCompleteTweenDelegate> onComplete)
        {
            parallelWriter.AddComponent(sortKey, entity, new MoveOrder(start,end,duration,onComplete));
        }

        public static float GetTime()
        {
            return Time.time;
        }
        
    }
}
