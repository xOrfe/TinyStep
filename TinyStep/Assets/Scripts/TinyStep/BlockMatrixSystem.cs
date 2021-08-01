using Unity.Entities;
using Tween = TinyStep.Tweener.Tweener;

namespace TinyStep
{
    public class BlockMatrixSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private EntityArchetype _blockAcheType;
        private Entity _blockMatrixEntity;


        protected override void OnUpdate()
        {
            
        }
    }
}
