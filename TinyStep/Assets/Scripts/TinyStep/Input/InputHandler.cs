using Unity.Entities;
using Unity.Transforms;

namespace TinyStep.Input
{
    public class InputHandler : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, in Rotation rotation) => {
                
            }).Schedule();
        }
    }
}
