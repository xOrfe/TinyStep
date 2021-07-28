using TinyStep.Utils;
using Unity.Entities;

namespace TinyStep.Input
{
    public class InputHandler : SystemBase
    {
        protected override void OnStartRunning()
        {
            RequireSingletonForUpdate<BlockMatrixData>();
            RequireSingletonForUpdate<InputData>();
        }

        protected override void OnUpdate()
        {
            var blockMatrixData = GetSingleton<BlockMatrixData>();
            if (blockMatrixData.MovingBlockCount > 0 && blockMatrixData.BlockCount != 81) return;
            
            var input = World.GetExistingSystem<Unity.Tiny.Input.InputSystem>();
            if (InputUtil.GetInputDown(input))
            {
                var pos = CameraUtil.ScreenPointToWorldPoint(World, InputUtil.GetInputPosition(input));
                var activeInput = GetSingleton<InputData>();
                activeInput.Pos = pos;
                activeInput.IsTouchExecuted = false;
                SetSingleton(activeInput);
            }
        }
    }
}
