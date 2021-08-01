using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TinyStep
{
    [GenerateAuthoringComponent]
    public struct BlockMatrixData : IComponentData
    {
        
        //Waiting for something is too important for managing structural changes.
        public bool IsWaitingForSomething => IsWaitingForBlocksCreate ||
                                             IsWaitingForDestroyedBlock ||
                                             IsWaitingForFallDown ||
                                             IsWaitingForMovingBlock ||
                                             IsWaitingForInputHandle ||
                                             IsWaitingForSetCrews;
        
        public bool CanIExecuteSetFallDown => !(IsWaitingForBlocksCreate ||
                                               IsWaitingForMovingBlock ||
                                               IsWaitingForInputHandle ||
                                               IsWaitingForSetCrews);
        
        public bool CanIExecuteBlockSpawn => !(IsWaitingForFallDown ||
                                               IsWaitingForMovingBlock ||
                                               IsWaitingForInputHandle ||
                                               IsWaitingForSetCrews);
        
        public bool CanIExecuteSetCrews => !(IsWaitingForBlocksCreate ||
                                             IsWaitingForDestroyedBlock ||
                                             IsWaitingForFallDown ||
                                             IsWaitingForInputHandle ||
                                             IsWaitingForMovingBlock);
        public bool CanIExecuteInputHandle => !(IsWaitingForBlocksCreate ||
                                              IsWaitingForDestroyedBlock ||
                                              IsWaitingForFallDown ||
                                              IsWaitingForMovingBlock ||
                                              IsWaitingForSetCrews);
        
        public BlockMatrixDefinition BlockMatrixDefinition;
        public int BlockCount => BlockMatrixDefinition.MatrixLength;

        //-------
        public int _destroyedBlockCount;
        public int DestroyedBlockCount => _destroyedBlockCount;
        public void BlockSpawned() => _destroyedBlockCount--;
        public void BlockSpawned(int count) => _destroyedBlockCount -= count;
        public void BlockDestroyed() => _destroyedBlockCount++;
        public void BlockDestroyed(int count) => _destroyedBlockCount += count;
        public bool IsWaitingForDestroyedBlock => _destroyedBlockCount > 0;
        //-------
        
        
        
        //-------
        public int _movingBlockCount;
        public int MovingBlockCount => _movingBlockCount;
        public bool IsWaitingForMovingBlock => _movingBlockCount > 0;
        public void BlockStartMoving() => _movingBlockCount++;
        public void BlockStartMoving(int count) => _movingBlockCount += count;
        public void BlockStopMoving() => _movingBlockCount--;
        public void BlockStopMoving(int count) => _movingBlockCount-=count;
        //-------
        
        
        
        //-------
        public bool _isWaitingForInputHandle;
        public bool IsWaitingForInputHandle => _isWaitingForInputHandle;
        public void StartInputHandle(){_isWaitingForInputHandle = true;}
        public void CompleteInputHandle(){_isWaitingForInputHandle = false;}
        //-------
        
        
        
        //-------
        public bool _isWaitingForFallDown;
        public bool IsWaitingForFallDown => _isWaitingForFallDown;
        public void StartFallDown(){_isWaitingForFallDown = true;}
        public void CompleteFallDown(){_isWaitingForFallDown = false;}
        //-------
        
        
        
        //-------
        public bool _isWaitingForBlocksCreate;
        public bool IsWaitingForBlocksCreate => _isWaitingForBlocksCreate;
        public void StartBlocksCreate(){_isWaitingForBlocksCreate = true;}
        public void CompleteBlocksCreate(){_isWaitingForBlocksCreate = false;}
        //-------
        
        
        
        //-------
        public bool _isWaitingForSetCrews;
        public bool IsWaitingForSetCrews => _isWaitingForSetCrews;
        public void StartSetCrews() => _isWaitingForSetCrews = true;
        public void CompleteSetCrews() => _isWaitingForSetCrews = false;
        //-------
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
