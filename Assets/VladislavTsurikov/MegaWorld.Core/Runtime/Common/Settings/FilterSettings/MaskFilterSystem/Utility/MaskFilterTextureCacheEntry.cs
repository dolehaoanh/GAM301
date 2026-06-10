using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.UnityUtility.Runtime;
using CommonGetFitness = VladislavTsurikov.MegaWorld.Runtime.Common.Utility.GetFitness;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public class MaskFilterTextureCacheEntry
    {
        private readonly Dictionary<Terrain, Texture2D> _terrainTextures = new();

        public readonly MaskFilterComponentSettings MaskFilterComponentSettings;

        public MaskFilterTextureCacheEntry(MaskFilterComponentSettings maskFilterComponentSettings) =>
            MaskFilterComponentSettings = maskFilterComponentSettings;

        public float GetFitness(Vector3 point)
        {
            return GetFitness(UnityTerrainUtility.GetTerrain(point), point);
        }

        public void InvalidateTerrain(Terrain terrain)
        {
            _terrainTextures.Remove(terrain);
        }

        public float GetFitness(Terrain terrain, Vector3 point)
        {
            if (terrain == null)
            {
                return 0;
            }

            Bounds terrainBounds = GetTerrainBounds(terrain);

            if (_terrainTextures.TryGetValue(terrain, out Texture2D mask))
            {
                return CommonGetFitness.GetFromMaskFilter(terrainBounds, MaskFilterComponentSettings.MaskFilterStack,
                    mask, point);
            }

            RayHit terrainCenterRayHit = RaycastUtility.Raycast(
                RayUtility.GetRayDown(terrainBounds.center + new Vector3(0, 20, 0)),
                LayerMask.GetMask(LayerMask.LayerToName(terrain.gameObject.layer)));

            if (terrainCenterRayHit == null)
            {
                return 0;
            }

            BoxArea area = new(terrainCenterRayHit, terrainBounds.size.x);
            Texture2D texture2D = FilterMaskOperation.UpdateMaskTexture(MaskFilterComponentSettings, area);
            _terrainTextures.Add(terrain, texture2D);

            return CommonGetFitness.GetFromMaskFilter(terrainBounds, MaskFilterComponentSettings.MaskFilterStack,
                texture2D, point);
        }

        private static Bounds GetTerrainBounds(Terrain terrain)
        {
            TerrainData data = terrain.terrainData;
            return new Bounds(data.bounds.center + terrain.transform.position, data.bounds.size);
        }
    }
}
