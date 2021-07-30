using System;
using TinyStep.Utils;
using Unity.Entities;

namespace TinyStep.DynamicBuffers
{
    [InternalBufferCapacity(81)]
    public struct BlockDefinitionBuffer : IBufferElementData
    {
        public Entity Entity;
        public bool Existence;
        public int PosIndex;
        public BlockType BlockType;
        public BlockColor BlockColor;
        public BlockObstacleType BlockObstacleType;

        public BlockDefinitionBuffer(Entity entity,int posIndex,int blockColor)
        {
            Entity = entity;
            Existence = true;
            PosIndex = posIndex;
            BlockType = BlockType.Block;
            BlockColor = (BlockColor)blockColor;
            BlockObstacleType = BlockObstacleType.Empty;
        }
        public BlockDefinitionBuffer(bool existence)
        {
            Entity = Entity.Null;
            Existence = existence;
            PosIndex = -1;
            BlockType = BlockType.Block;
            BlockColor = BlockColor.Empty;
            BlockObstacleType = BlockObstacleType.Empty;
        }
    }
    public enum BlockType
    {
        Block,
        Ability
    }
    public enum BlockColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Empty
    }
    public enum BlockObstacleType
    {
        Rocket,
        Bomb,
        Ulti,
        Empty
    }
}
