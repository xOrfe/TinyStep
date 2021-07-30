using System;
using Unity.Collections;
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

        private bool _isWaitingForFallDown;
        public bool IsWaitingForFallDown => _isWaitingForFallDown;
        public void StartFallDown(){_isWaitingForFallDown = true;}
        public void CompleteFallDown(){_isWaitingForFallDown = false;}
        
        private bool _isWaitingForSetCrews;
        public bool IsWaitingForSetCrews => _isWaitingForFallDown;
        public void StartSetCrews(){_isWaitingForSetCrews = true;}
        public void CompleteSetCrews(){_isWaitingForSetCrews = false;}
        public void BlockSpawned(){_destroyedBlockCount--;}
        public void BlockDestroyed(){_destroyedBlockCount++;}
        public void BlockStartMoving(){_movingBlockCount++;}
        public void BlockStartMoving(int count){_movingBlockCount+=count;}
        public void BlockStopMoving(){_movingBlockCount--;}
        public void BlockStopMoving(int count){_movingBlockCount-=count;}
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
