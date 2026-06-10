using UnityEngine;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public sealed class UnityTerrainBuildResult
    {
        public string AssetFolderPath;
        public GameObject Container;
        public int ImportedMaskCount;
        public int ImportedTileCount;
        public TerrainSync Sync;
    }
}
