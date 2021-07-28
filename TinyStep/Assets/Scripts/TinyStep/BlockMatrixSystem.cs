using TinyStep.Tweener;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;
using AOT;
namespace TinyStep
{
    public class BlockMatrixSystem : SystemBase
    {
        private BlockMatrixData _blockMatrixData;
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _blockMatrixData = GetSingleton<BlockMatrixData>();
            RequireSingletonForUpdate<BlockMatrixData>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnStartRunning()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.
                WithAll<Block>().
                ForEach((Entity entity,in Translation translation) => {
                    FunctionPointer<OnCompleteTweenDelegate> onComplete = BurstCompiler.CompileFunctionPointer<OnCompleteTweenDelegate>(OnCompleteTween);
                    Tweener.Tweener.Move(ecb,0, entity, translation.Value, new float3(20.0f, 20.0f, 0), 1.0f,onComplete);
                    
                }).Schedule();
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        protected override void OnUpdate()
        {
            
        }
        
        [BurstCompile]
        [MonoPInvokeCallback(typeof(OnCompleteTweenDelegate))]
        public void OnCompleteTween()
        {
            var blockMatrixData = new BlockMatrixData()
            {
                BlockCount = _blockMatrixData.BlockCount,
                MovingBlockCount =  _blockMatrixData.MovingBlockCount - 1
            };
            
            SetSingleton(blockMatrixData);
            
            _blockMatrixData = GetSingleton<BlockMatrixData>();
        }
        public delegate void OnCompleteTweenDelegate ();

    }
}
