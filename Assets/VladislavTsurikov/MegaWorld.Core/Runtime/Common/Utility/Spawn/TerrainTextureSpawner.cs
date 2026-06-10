using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;

#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn
{
    public static class TerrainTextureSpawner
    {
        public static void SpawnArea(Group group, IReadOnlyList<Prototype> prototypeTerrainTextures,
            BoxArea boxArea, float textureTargetStrength, Terrain terrain)
        {
            if (terrain == null)
            {
                return;
            }

            if (TerrainResourcesController.IsSyncError(group, terrain))
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new(boxArea, terrain);

            foreach (PrototypeTerrainTexture proto in prototypeTerrainTextures)
            {
                if (proto.Active)
                {
                    SpawnPrototype(proto, terrainPainterRenderHelper, textureTargetStrength);
                }
            }
        }

        private static void SpawnPrototype(PrototypeTerrainTexture proto,
            TerrainPainterRenderHelper terrainPainterRenderHelper, float textureTargetStrength)
        {
            MaskFilterComponentSettings maskFilterComponentSettings =
                (MaskFilterComponentSettings)proto.GetElement(typeof(MaskFilterComponentSettings));

            PaintContext paintContext = terrainPainterRenderHelper.AcquireTexture(proto.TerrainLayer);

            if (paintContext != null)
            {
                FilterMaskOperation.UpdateFilterContext(ref maskFilterComponentSettings.FilterContext,
                    maskFilterComponentSettings.MaskFilterStack, terrainPainterRenderHelper.BoxArea);

                RenderTexture filterMaskRT = maskFilterComponentSettings.FilterContext.Output;

                Material mat = MaskFilterUtility.GetPaintMaterial();

                float targetAlpha = textureTargetStrength;
                float s = 1;
                Vector4 brushParams = new(s, targetAlpha, 0.0f, 0.0f);
                mat.SetTexture("_BrushTex", terrainPainterRenderHelper.BoxArea.Mask);
                mat.SetVector("_BrushParams", brushParams);
                mat.SetTexture("_FilterTex", filterMaskRT);

                terrainPainterRenderHelper.SetupTerrainToolMaterialProperties(paintContext, mat);

                terrainPainterRenderHelper.RenderBrush(paintContext, mat, 0);

                TerrainPaintUtility.EndPaintTexture(paintContext, "Terrain Paint - Texture");

                if (maskFilterComponentSettings.FilterContext != null)
                {
                    maskFilterComponentSettings.FilterContext.Dispose();
                }
            }
        }
    }
}
