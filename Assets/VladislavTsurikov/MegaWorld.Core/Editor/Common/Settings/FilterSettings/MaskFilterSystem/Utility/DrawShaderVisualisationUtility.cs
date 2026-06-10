#if UNITY_EDITOR
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    public static class DrawShaderVisualisationUtility
    {
        private static Material _brushPreviewMaterial;

        private static readonly int _alphaVisualisationType = Shader.PropertyToID("_AlphaVisualisationType");
        private static readonly int _alpha = Shader.PropertyToID("_Alpha");
        private static readonly int _colorSpace = Shader.PropertyToID("_ColorSpace");
        private static readonly int _enableBrushStripe = Shader.PropertyToID("_EnableBrushStripe");
        private static readonly int _color = Shader.PropertyToID("_Color");
        private static readonly int _blendParams = Shader.PropertyToID("_BlendParams");
        private static readonly int _blendTex = Shader.PropertyToID("_BlendTex");

        private static Material BrushPreviewMaterial
        {
            get
            {
                if (_brushPreviewMaterial == null)
                {
                    Shader shader = Shader.Find("MegaWorld/TerrainEngine/BrushPreview");
                    if (shader != null)
                    {
                        _brushPreviewMaterial = new Material(shader);
                    }
                }

                return _brushPreviewMaterial;
            }
        }

        public static void DrawAreaPreview(BoxArea boxArea)
        {
            if (boxArea.TerrainUnder == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new(boxArea);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();

            if (heightContext == null)
            {
                return;
            }

            Material previewMaterial = BrushPreviewMaterial != null
                ? BrushPreviewMaterial
                : TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();

#if UNITY_2021_2_OR_NEWER
            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainBrushPreviewMode.SourceRenderTexture,
                previewMaterial, 0);
#else
            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, previewMaterial, 0);
#endif

            TerrainPaintUtility.ReleaseContextResources(heightContext);
        }

        public static void DrawMaskFilter(BoxArea boxArea, MaskFilterContext filterContext, float multiplyAlpha = 1)
        {
            if (boxArea.TerrainUnder == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new(boxArea);

            Texture brushTexture = terrainPainterRenderHelper.BoxArea.Mask;

            Material brushMaterial = MaskFilterUtility.GetBrushPreviewMaterial();
            RenderTexture tmpRT = RenderTexture.active;

            RenderTexture filterMaskRT = filterContext.Output;

            if (filterMaskRT != null)
            {
                VisualisationMaskFiltersPreference visualisationMaskFiltersPreference =
                    PreferenceElementSingleton<VisualisationMaskFiltersPreference>.Instance;

                //Composite the brush texture onto the filter stack result
                RenderTexture compRT = RenderTexture.GetTemporary(filterMaskRT.descriptor);
                Material blendMat = MaskFilterUtility.GetBlendMaterial();
                blendMat.SetTexture(_blendTex, brushTexture);
                blendMat.SetVector(_blendParams,
                    new Vector4(0.0f, 0.0f, -(terrainPainterRenderHelper.BoxArea.Rotation * Mathf.Deg2Rad), 0.0f));
                TerrainPaintUtility.SetupTerrainToolMaterialProperties(filterContext.HeightContext,
                    terrainPainterRenderHelper.BrushTransform, blendMat);
                Graphics.Blit(filterMaskRT, compRT, blendMat, 0);

                RenderTexture.active = tmpRT;

                brushMaterial.SetColor(_color, visualisationMaskFiltersPreference.Color);
                brushMaterial.SetInt(_enableBrushStripe, visualisationMaskFiltersPreference.EnableStripe ? 1 : 0);
                brushMaterial.SetInt(_colorSpace, (int)visualisationMaskFiltersPreference.ColorSpace);
                brushMaterial.SetInt(_alphaVisualisationType,
                    (int)visualisationMaskFiltersPreference.AlphaVisualisationType);
                brushMaterial.SetFloat(_alpha, visualisationMaskFiltersPreference.CustomAlpha * multiplyAlpha);

                TerrainPaintUtility.SetupTerrainToolMaterialProperties(filterContext.HeightContext,
                    terrainPainterRenderHelper.BrushTransform, brushMaterial);
                TerrainPaintUtilityEditor.DrawBrushPreview(filterContext.HeightContext,
                    TerrainBrushPreviewMode.SourceRenderTexture, compRT, terrainPainterRenderHelper.BrushTransform,
                    brushMaterial, 0);
                RenderTexture.ReleaseTemporary(compRT);
            }
        }
    }
}
#endif
