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
            in float duration)
        {
            parallelWriter.AddComponent(sortKey, entity, new MoveOrder(start,end,duration));
        }
        public static void Move(
            in EntityCommandBuffer entityCommandBuffer, 
            in Entity entity, 
            in float3 start, 
            in float3 end, 
            in float duration)
        {
            entityCommandBuffer.AddComponent(entity,new MoveOrder(start,end,duration) );
        }
        public static float GetTime()
        {
            return Time.time;
        }
        
    }
}
