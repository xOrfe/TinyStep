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
        public int BlockCount => BlockMatrixDefinition.MatrixLength;

        private int _destroyedBlockCount;
        public int DestroyedBlockCount => _destroyedBlockCount;

        private int _movingBlockCount;
        public int MovingBlockCount => _movingBlockCount;
        
        public void BlockSpawned(){_destroyedBlockCount--;}
        public void BlockDestroyed(){_destroyedBlockCount++;}
        public void ABlockStartMoving(){_movingBlockCount++;}
        public void ABlockStopMoving(){_movingBlockCount--;}
    }
    
    [Serializable]
    public struct BlockMatrixDefinition
    {
        [SerializeField] private int2 matrixScale;
        public int2 MatrixScale => matrixScale;
        public int MatrixLength => matrixScale.x * matrixScale.y;
        
        [SerializeField] private float oneBlockScale;
        public float OneBlockScale => oneBlockScale;
    }
}
