using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public static class MaskFilterTextureCache
    {
        private static readonly List<MaskFilterTextureCacheEntry> EntryList = new();

        static MaskFilterTextureCache()
        {
            TerrainCallbacks.heightmapChanged += OnHeightmapChanged;
            TerrainCallbacks.textureChanged += OnTextureChanged;
        }

        private static void OnHeightmapChanged(Terrain terrain, RectInt heightRegion, bool synched)
        {
            foreach (MaskFilterTextureCacheEntry entry in EntryList)
            {
                entry.InvalidateTerrain(terrain);
            }
        }

        private static void OnTextureChanged(Terrain terrain, string textureName, RectInt texelRegion, bool synched)
        {
            foreach (MaskFilterTextureCacheEntry entry in EntryList)
            {
                entry.InvalidateTerrain(terrain);
            }
        }

        public static float GetFitness(MaskFilterComponentSettings maskFilterComponentSettings, Vector3 point)
        {
            return GetOrCreate(maskFilterComponentSettings).GetFitness(point);
        }

        public static float GetFitness(MaskFilterComponentSettings maskFilterComponentSettings, Terrain terrain,
            Vector3 point)
        {
            return GetOrCreate(maskFilterComponentSettings).GetFitness(terrain, point);
        }

        private static MaskFilterTextureCacheEntry GetOrCreate(MaskFilterComponentSettings settings)
        {
            foreach (MaskFilterTextureCacheEntry entry in EntryList)
            {
                if (entry.MaskFilterComponentSettings == settings)
                {
                    return entry;
                }
            }

            MaskFilterTextureCacheEntry newEntry = new(settings);
            EntryList.Add(newEntry);
            return newEntry;
        }

        public static void Clear()
        {
            EntryList.Clear();
        }
    }
}
