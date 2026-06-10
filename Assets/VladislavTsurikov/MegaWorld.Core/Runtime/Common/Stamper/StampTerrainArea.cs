using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public sealed class StampTerrainArea
    {
        public Terrain Terrain { get; }
        public Bounds Bounds { get; }
        public Bounds TerrainBounds { get; }
        public RayHit RayHit { get; }

        public StampTerrainArea(Terrain terrain, Bounds bounds, Bounds terrainBounds, RayHit rayHit)
        {
            Terrain = terrain;
            Bounds = bounds;
            TerrainBounds = terrainBounds;
            RayHit = rayHit;
        }
    }
}
