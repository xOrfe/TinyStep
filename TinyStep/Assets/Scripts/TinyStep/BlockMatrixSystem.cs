using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace TinyStep
{
    public class BlockMatrixSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.
                WithAll<Block>().
                ForEach((ref Translation translation) => {
                    Debug.Log(translation.Value);
                    translation.Value += new float3(1, 0, 0);
                }).Schedule();
        }
    }
}
