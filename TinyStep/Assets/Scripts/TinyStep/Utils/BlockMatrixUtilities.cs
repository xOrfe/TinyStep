using Unity.Burst;
using Unity.Mathematics;

namespace TinyStep.Utils
{
    public static class BlockMatrixUtilities
    {
        public static float3 GetBlockPositionLocal(int index,int2 matrixScale,float oneBlockScale)
        {
            return GetBlockPositionLocal(GetMatrixCoordFromIndex(index,matrixScale),matrixScale,oneBlockScale);
        }
        public static float3 GetBlockPositionLocal(int2 matrixPos,int2 matrixScale,float oneBlockScale)
        {
            float3 normalPos = new float3(matrixPos.x * oneBlockScale + oneBlockScale / 2,
                                   matrixPos.y * oneBlockScale + oneBlockScale / 2, 0)
                               - new float3(matrixScale.x, matrixScale.y, 0) * (oneBlockScale / 2);

            return normalPos;
        }
        public static int2 GetMatrixCoordFromIndex(int index,int2 matrixScale)
        {
            int x = index % matrixScale.x;
            int y = (index - x) / matrixScale.x;
            return new int2(x, y);
        }
        
    }
}
