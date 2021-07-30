using AOT;
using Unity.Burst;
using Unity.Mathematics;

namespace TinyStep.Tweener
{
    //From xOrfe.Entityween repo

    [BurstCompile]
    public class EaseJobs
    {
        [BurstCompile]
        [MonoPInvokeCallback(typeof(EaseTypeDelegate))]
        public static float InSine(float value)
        {
            return math.sin(value * (math.PI * 0.5f));
        }


        [BurstCompile]
        [MonoPInvokeCallback(typeof(EaseTypeDelegate))]
        public static float Linear(float value)
        {
            return value;
        }

        public delegate float EaseTypeDelegate(float Value);
    }

    public class OnCompleteJobs
    {
        [BurstCompile]
        [MonoPInvokeCallback(typeof(OnCompleteDelegate))]
        public static void BlockMoveOnComplete()
        {
            
        }
        public delegate void OnCompleteDelegate();
    }

    [BurstCompile]
    public class ActionJobs
    {
        [BurstCompile]
        [MonoPInvokeCallback(typeof(ActionTypeDelegate))]
        public static void Float3To(ref float3 trns, ref float3 start, ref float3 end, float percentage)
        {
            trns = math.lerp(start, end, percentage);
        }

        public delegate void ActionTypeDelegate(ref float3 trns, ref float3 changeStarts, ref float3 changeFactors,
            float progress);
    }
}