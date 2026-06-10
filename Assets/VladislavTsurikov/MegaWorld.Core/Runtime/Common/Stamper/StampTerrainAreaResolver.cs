using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public sealed class StampTerrainAreaResolver
    {
        private const int StampBoundsPaddingPixels = 1;

        private readonly List<BoxArea> _stampAreas = new();

        public IReadOnlyList<BoxArea> StampAreas => _stampAreas;

        public void RefreshCells(Bounds globalBounds, bool addTexturePixelPadding = true)
        {
            _stampAreas.Clear();

            if (Terrain.activeTerrains == null || Terrain.activeTerrains.Length == 0)
            {
                return;
            }

            for (int terrainIndex = 0; terrainIndex < Terrain.activeTerrains.Length; terrainIndex++)
            {
                Terrain terrain = Terrain.activeTerrains[terrainIndex];
                if (terrain == null || terrain.terrainData == null)
                {
                    continue;
                }

                Bounds terrainBounds = new(terrain.terrainData.bounds.center + terrain.transform.position,
                    terrain.terrainData.bounds.size);

                if (!IntersectsOnXZ(globalBounds, terrainBounds))
                {
                    continue;
                }

                Bounds stampBounds = CreateIntersectionBounds(globalBounds, terrainBounds);
                if (stampBounds.size.x <= 0f || stampBounds.size.z <= 0f)
                {
                    continue;
                }

                Bounds bounds = addTexturePixelPadding
                    ? AddTexturePixelPadding(stampBounds, terrain, StampBoundsPaddingPixels)
                    : stampBounds;

                Vector3 centerPoint = bounds.center;
                centerPoint.y = terrain.SampleHeight(centerPoint) + terrain.transform.position.y;

                RayHit rayHit = new(terrain, Vector3.up, centerPoint, 0f);
                BoxArea boxArea = new(rayHit, bounds.size, terrain)
                    { TerrainBounds = terrainBounds };

                _stampAreas.Add(boxArea);
            }
        }

        private static Bounds CreateIntersectionBounds(Bounds globalBounds, Bounds terrainBounds)
        {
            float minX = Mathf.Max(globalBounds.min.x, terrainBounds.min.x);
            float maxX = Mathf.Min(globalBounds.max.x, terrainBounds.max.x);
            float minZ = Mathf.Max(globalBounds.min.z, terrainBounds.min.z);
            float maxZ = Mathf.Min(globalBounds.max.z, terrainBounds.max.z);

            Vector3 min = new(minX, terrainBounds.min.y, minZ);
            Vector3 max = new(maxX, terrainBounds.max.y, maxZ);

            Bounds intersectionBounds = new();
            intersectionBounds.SetMinMax(min, max);
            return intersectionBounds;
        }

        private static bool IntersectsOnXZ(Bounds first, Bounds second)
        {
            bool intersectsX = first.min.x < second.max.x && first.max.x > second.min.x;
            bool intersectsZ = first.min.z < second.max.z && first.max.z > second.min.z;
            return intersectsX && intersectsZ;
        }

        // Adds a one-alphamap-pixel overlap so split terrain paint areas do not leave seams on tile borders.
        private static Bounds AddTexturePixelPadding(Bounds bounds, Terrain terrain, int pixelCount)
        {
            if (terrain == null || terrain.terrainData == null || pixelCount <= 0)
            {
                return bounds;
            }

            TerrainData terrainData = terrain.terrainData;
            float pixelSizeX = terrainData.size.x / terrainData.alphamapWidth;
            float pixelSizeZ = terrainData.size.z / terrainData.alphamapHeight;

            bounds.Expand(new Vector3(pixelSizeX * pixelCount * 2f, 0f, pixelSizeZ * pixelCount * 2f));
            return bounds;
        }
    }
}
