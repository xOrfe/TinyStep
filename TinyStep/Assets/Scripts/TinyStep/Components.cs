
using Unity.Entities;
namespace TinyStep
{
    public struct BlockSpawner : IComponentData
    {
        public Entity Prefab;
    }
    public struct BlockSprite : IBufferElementData
    {
        public Entity Sprite;
    }
}