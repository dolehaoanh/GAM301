using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public class StamperTerrainArea : Area
    {
        [NonSerialized]
        private StampTerrainAreaResolver _stampTerrainAreaResolver;

        public IReadOnlyList<BoxArea> StampAreas => _stampTerrainAreaResolver.StampAreas;

        protected override void SetupArea()
        {
            _stampTerrainAreaResolver = new StampTerrainAreaResolver();

            OnSetAreaBounds -= RefreshCells;
            OnSetAreaBounds += RefreshCells;

#if UNITY_EDITOR
            OnSetAreaBounds -= MarkVisualisationDirty;
            OnSetAreaBounds += MarkVisualisationDirty;
#endif
        }

        public void RefreshCells()
        {
            RefreshCells(true);
        }

        public void RefreshCells(bool addTexturePixelPadding)
        {
            _stampTerrainAreaResolver.RefreshCells(Bounds, addTexturePixelPadding);

            Texture2D mask = GetCurrentRaw();
            foreach (BoxArea area in _stampTerrainAreaResolver.StampAreas)
            {
                area.Mask = mask;
            }
        }

        protected virtual void MarkVisualisationDirty()
        {
        }
    }
}
