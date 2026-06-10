using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.UnityUtility.Runtime;
#if UNITY_EDITOR
using UnityEditor.TerrainTools;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public class TerrainPainterRenderHelper
    {
        private readonly Rect _brushRect;
        private readonly BrushTransform _brushTransform;
        private readonly Terrain _terrainUnderCursor;

        public BoxArea BoxArea { get; }
        public BrushTransform BrushTransform => _brushTransform;

        public TerrainPainterRenderHelper(BoxArea boxArea)
            : this(boxArea, boxArea.TerrainUnder != null
                ? boxArea.TerrainUnder
                : UnityTerrainUtility.GetTerrain(boxArea.Center))
        {
        }

        public TerrainPainterRenderHelper(BoxArea boxArea, Terrain terrain)
        {
            BoxArea = boxArea;
            _terrainUnderCursor = terrain;

            Vector2 currUV = UnityTerrainUtility.WorldPointToUV(boxArea.Center, _terrainUnderCursor);

            _brushTransform =
                CalculateBrushTransform(_terrainUnderCursor, currUV,
                    new Vector2(boxArea.LocalSize.x, boxArea.LocalSize.z));
            _brushRect = _brushTransform.GetBrushXYBounds();
        }

        public static BrushTransform CalculateBrushTransform(
            Terrain terrain,
            Vector2 brushCenterTerrainUV,
            Vector2 brushSize)
        {
            Vector2 brushU = Vector2.right * brushSize.x;
            Vector2 brushV = Vector2.up * brushSize.y;

            Vector3 terrainSize = terrain.terrainData.size;

            Vector2 terrainCenter = brushCenterTerrainUV * new Vector2(
                terrainSize.x,
                terrainSize.z);

            Vector2 origin = terrainCenter - 0.5f * brushU - 0.5f * brushV;

            return new BrushTransform(origin, brushU, brushV);
        }

        public void RenderBrush(PaintContext paintContext, Material material, int pass)
        {
            Texture sourceTexture = paintContext.sourceRenderTexture;
            RenderTexture destinationTexture = paintContext.destinationRenderTexture;

            Graphics.Blit(sourceTexture, destinationTexture, material, pass);
        }

#if UNITY_EDITOR
        public void RenderAreaPreview(PaintContext paintContext, TerrainBrushPreviewMode previewTexture,
            Material material, int pass)
        {
            TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, BoxArea.Mask, _brushTransform,
                material, pass);
        }
#endif

        public void SetupTerrainToolMaterialProperties(PaintContext paintContext, Material material)
        {
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, _brushTransform, material);
        }

        public PaintContext AcquireHeightmap(int extraBorderPixels = 0)
        {
            return TerrainPaintUtility.BeginPaintHeightmap(_terrainUnderCursor, _brushRect, extraBorderPixels);
        }

        public PaintContext AcquireTexture(TerrainLayer layer, int extraBorderPixels = 0)
        {
            return TerrainPaintUtility.BeginPaintTexture(_terrainUnderCursor, _brushRect, layer, extraBorderPixels);
        }

        public PaintContext AcquireNormalmap(int extraBorderPixels = 0)
        {
            return TerrainPaintUtility.CollectNormals(_terrainUnderCursor, _brushRect, extraBorderPixels);
        }

    }
}
