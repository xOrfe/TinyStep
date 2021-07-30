using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace TinyStep.Tweener
{
    [Serializable]
    public struct MoveOrder : IComponentData
    {
        public float3 start;
        
        public float3 end;
        
        public float duration;
        
        public float deltaTime;
        
        public FunctionPointer<OnCompleteJobs.OnCompleteDelegate> OnComplete;
        
        public MoveOrder(in float3 _start, in float3 _end, in float _duration)
        {
            start = _start;
            end = _end;
            duration = _duration;
            deltaTime = 0;
            OnComplete = BurstCompiler.CompileFunctionPointer<OnCompleteJobs.OnCompleteDelegate>(OnCompleteJobs.BlockMoveOnComplete);
        }
    }
}
