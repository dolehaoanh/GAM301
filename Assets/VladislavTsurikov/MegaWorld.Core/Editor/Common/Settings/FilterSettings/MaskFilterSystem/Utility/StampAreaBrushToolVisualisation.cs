#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    public static class StampAreaBrushToolVisualisation
    {
        private static readonly Dictionary<Terrain, MaskFilterVisualisation> _visByTerrain = new();

        public static void Draw(BoxArea fullArea, IReadOnlyList<BoxArea> stampAreas, Texture2D proceduralMask,
            SelectionData data)
        {
            if (fullArea == null || fullArea.RayHit == null || proceduralMask == null)
            {
                return;
            }

            foreach (BoxArea stampArea in stampAreas)
            {
                TerrainPainterRenderHelper helper = new(stampArea);
                PaintContext heightContext = helper.AcquireHeightmap();
                if (heightContext == null)
                {
                    continue;
                }

                BrushMaskCropUtility.GetCropUVWindow(fullArea, stampArea, out Vector2 centerUV, out Vector2 sizeUV);
                RenderTexture croppedRT =
                    BrushMaskCropUtility.CropGPU(proceduralMask, heightContext, helper.BrushTransform, centerUV, sizeUV);
                TerrainPaintUtility.ReleaseContextResources(heightContext);

                stampArea.Mask = croppedRT;

                if (data.SelectedData.HasOneSelectedPrototype())
                {
                    if (!_visByTerrain.TryGetValue(stampArea.TerrainUnder, out MaskFilterVisualisation vis))
                    {
                        vis = new MaskFilterVisualisation();
                        _visByTerrain[stampArea.TerrainUnder] = vis;
                    }

                    vis.DrawMaskFilterVisualization(MaskFilterUtility.GetMaskFilterFromSelectedPrototype(data), stampArea);
                }
                else
                {
                    DrawShaderVisualisationUtility.DrawAreaPreview(stampArea);
                }

                RenderTexture.ReleaseTemporary(croppedRT);
            }
        }
    }
}
#endif
