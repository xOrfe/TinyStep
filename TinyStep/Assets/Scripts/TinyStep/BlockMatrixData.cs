using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TinyStep
{
    [GenerateAuthoringComponent]
    public struct BlockMatrixData : IComponentData
    {
        public BlockMatrixDefinition BlockMatrixDefinition;
        
        private int _blockCount;
        public int BlockCount => _blockCount;

        private int _movingBlockCount;
        public int MovingBlockCount => _movingBlockCount;
        
        public void BlockSpawned(){_blockCount++;}
        public void BlockDestroyed(){_blockCount--;}
        public void ABlockStartMoving(){_movingBlockCount++;}
        public void ABlockStopMoving(){_movingBlockCount--;}
    }
    
    [Serializable]
    public struct BlockMatrixDefinition
    {
        [SerializeField] private int2 matrixScale;
        public int2 MatrixScale => matrixScale;
        
        [SerializeField] private float oneBlockScale;
        public float OneBlockScale => oneBlockScale;
    }
}
