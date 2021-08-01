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
        public static float2 GetBlockPositionLocal(float2 blockCoord,float oneBlockScale)
        {
            float2 normalPos = new float2(blockCoord.x * oneBlockScale + oneBlockScale / 2,
                                   blockCoord.y * oneBlockScale + oneBlockScale / 2)
                                - new float2(blockCoord.x, blockCoord.y) * (oneBlockScale / 2);
            return normalPos;
        }
        public static int GetIndexFromWorld(float2 pos,float oneBlockScale,int2 matrixScale)
        {
            float2 click = (pos + new float2(matrixScale.x, matrixScale.y) * (oneBlockScale / 2)) / oneBlockScale;

            int2 tempPos = new int2((int)math.floor(math.abs(click.x)), (int)math.floor(math.abs(click.y)));

            int2 hitMatrixPos = new int2(tempPos.x, tempPos.y);

            int hitIndex = GetIndexFromMatrixCoord(hitMatrixPos,matrixScale);
            return hitIndex;
        }
        public static int GetIndexFromMatrixCoord(int2 matrixCoord,int2 matrixScale)
        {
            return (matrixCoord.y * matrixScale.x) + matrixCoord.x;
        }
    }
}
