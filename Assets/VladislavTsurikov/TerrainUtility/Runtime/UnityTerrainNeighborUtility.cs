using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public static class UnityTerrainNeighborUtility
    {
        public static void ApplyNeighbors(Dictionary<Vector2Int, Terrain> terrains, int tilesX, int tilesY)
        {
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    Vector2Int key = new(x, y);
                    if (!terrains.TryGetValue(key, out Terrain current))
                    {
                        continue;
                    }

                    terrains.TryGetValue(new Vector2Int(x - 1, y), out Terrain left);
                    terrains.TryGetValue(new Vector2Int(x + 1, y), out Terrain right);
                    terrains.TryGetValue(new Vector2Int(x, y + 1), out Terrain top);
                    terrains.TryGetValue(new Vector2Int(x, y - 1), out Terrain bottom);

                    current.SetNeighbors(left, top, right, bottom);
                }
            }
        }
    }
}
