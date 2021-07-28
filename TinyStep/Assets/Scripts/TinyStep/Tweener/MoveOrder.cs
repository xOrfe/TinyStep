using System;
using TinyStep.Tweener;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TinyStep
{
    [Serializable]
    public struct MoveOrder : IComponentData
    {
        public float3 start;
        
        public float3 end;
        
        public float duration;
        
        public float startTime;
        
        public MoveOrder(in float3 _start, in float3 _end, in float _duration)
        {
            start = _start;
            end = _end;
            duration = _duration;
            startTime = Tweener.Tweener.GetTime();
        }
    }
    
    [Serializable] public struct MoveOrderOnComplete : IComponentData {}
    [Serializable] public struct SetCrews : IComponentData {}
    [Serializable] public struct SetFallDowns : IComponentData {}
    
}
