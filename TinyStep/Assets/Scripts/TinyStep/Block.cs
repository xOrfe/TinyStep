using Unity.Entities;

namespace TinyStep
{
    [GenerateAuthoringComponent]
    public struct Block : IComponentData
    {
    }

    public enum BlockColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }
}
