using System;
using UnityEngine;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    [Serializable]
    public sealed class AreaMaskEntry
    {
        public string Id = string.Empty;
        public Texture2D Mask;
    }
}
