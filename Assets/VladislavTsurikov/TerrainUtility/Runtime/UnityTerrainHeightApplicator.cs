using UnityEngine;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public static class UnityTerrainHeightApplicator
    {
        public static void ApplyHeights(TerrainData terrainData, float[,] sourceHeights)
        {
            int targetResolution = GetUnityHeightmapResolution(sourceHeights);
            float[,] unityHeights = CreateUnityHeightArray(sourceHeights, null, null, null, targetResolution);
            terrainData.heightmapResolution = targetResolution;
            terrainData.SetHeights(0, 0, unityHeights);
        }

        public static void ApplyHeights(TerrainData terrainData, float[,] sourceHeights, float[,] rightNeighborHeights,
            float[,] topNeighborHeights, float[,] topRightNeighborHeights)
        {
            int targetResolution = GetUnityHeightmapResolution(sourceHeights);
            float[,] unityHeights = CreateUnityHeightArray(sourceHeights, rightNeighborHeights, topNeighborHeights,
                topRightNeighborHeights, targetResolution);
            terrainData.heightmapResolution = targetResolution;
            terrainData.SetHeights(0, 0, unityHeights);
        }

        public static int GetUnityHeightmapResolution(float[,] sourceHeights)
        {
            int sourceResolution = sourceHeights.GetLength(0);
            return sourceResolution > 1 ? sourceResolution + 1 : sourceResolution;
        }

        public static float[,] CreateUnityHeightArray(float[,] sourceHeights, int targetResolution)
        {
            return CreateUnityHeightArray(sourceHeights, null, null, null, targetResolution);
        }

        public static float[,] CreateUnityHeightArray(float[,] sourceHeights, float[,] rightNeighborHeights,
            float[,] topNeighborHeights, float[,] topRightNeighborHeights, int targetResolution)
        {
            int sourceHeight = sourceHeights.GetLength(0);
            int sourceWidth = sourceHeights.GetLength(1);

            if (sourceHeight == targetResolution && sourceWidth == targetResolution)
            {
                return sourceHeights;
            }

            float[,] targetHeights = new float[targetResolution, targetResolution];

            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    targetHeights[y, x] = sourceHeights[y, x];
                }
            }

            int lastSourceY = sourceHeight - 1;
            int lastSourceX = sourceWidth - 1;
            int lastTarget = targetResolution - 1;

            for (int y = 0; y < sourceHeight; y++)
            {
                targetHeights[y, lastTarget] = rightNeighborHeights != null
                    ? rightNeighborHeights[y, 0]
                    : sourceHeights[y, lastSourceX];
            }

            for (int x = 0; x < sourceWidth; x++)
            {
                targetHeights[lastTarget, x] = topNeighborHeights != null
                    ? topNeighborHeights[0, x]
                    : sourceHeights[lastSourceY, x];
            }

            if (topRightNeighborHeights != null)
            {
                targetHeights[lastTarget, lastTarget] = topRightNeighborHeights[0, 0];
            }
            else if (rightNeighborHeights != null)
            {
                targetHeights[lastTarget, lastTarget] = rightNeighborHeights[lastSourceY, 0];
            }
            else if (topNeighborHeights != null)
            {
                targetHeights[lastTarget, lastTarget] = topNeighborHeights[0, lastSourceX];
            }
            else
            {
                targetHeights[lastTarget, lastTarget] = sourceHeights[lastSourceY, lastSourceX];
            }

            return targetHeights;
        }
    }
}
