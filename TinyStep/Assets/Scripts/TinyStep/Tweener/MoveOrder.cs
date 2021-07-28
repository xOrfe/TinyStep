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
        
        public FunctionPointer<BlockMatrixSystem.OnCompleteTweenDelegate> OnComplete;
        
        public FunctionPointer<EaseJobs.EaseTypeDelegate> MyEaseMethod;
        
        public FunctionPointer<ActionJobs.ActionTypeDelegate> MyActionMethod;
        
        public MoveOrder(in float3 _start, in float3 _end, in float _duration,FunctionPointer<BlockMatrixSystem.OnCompleteTweenDelegate> _onComplete)
        {
            start = _start;
            end = _end;
            duration = _duration;
            startTime = Tweener.Tweener.GetTime();
            OnComplete = _onComplete;
            MyEaseMethod = BurstCompiler.CompileFunctionPointer<EaseJobs.EaseTypeDelegate>(EaseJobs.Linear);
            MyActionMethod = BurstCompiler.CompileFunctionPointer<ActionJobs.ActionTypeDelegate>(ActionJobs.Float3To);
        }
    }
}
