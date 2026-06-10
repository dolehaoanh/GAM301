using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using FilterMode = UnityEngine.FilterMode;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility
{
    public static class BrushMaskCropUtility
    {
        private static Material _cropMaterial;
        private static Texture2D[] _cpuCache;
        private static Color[] _cpuPixelBuffer;

        public static void GetCropUVWindow(BoxArea fullArea, BoxArea stampArea, out Vector2 centerUV, out Vector2 sizeUV)
        {
            float centerU = Mathf.InverseLerp(fullArea.Bounds.min.x, fullArea.Bounds.max.x, stampArea.Center.x);
            float centerV = Mathf.InverseLerp(fullArea.Bounds.min.z, fullArea.Bounds.max.z, stampArea.Center.z);
            float sizeU = stampArea.LocalSize.x / fullArea.LocalSize.x;
            float sizeV = stampArea.LocalSize.z / fullArea.LocalSize.z;

            centerUV = new Vector2(Mathf.Clamp01(centerU), Mathf.Clamp01(centerV));
            sizeUV = new Vector2(Mathf.Clamp01(sizeU), Mathf.Clamp01(sizeV));
        }

        public static RenderTexture CropGPU(Texture2D sourceMask, PaintContext heightContext,
            BrushTransform brushTransform, Vector2 centerUV, Vector2 sizeUV)
        {
            if (_cropMaterial == null)
            {
                _cropMaterial = new Material(Shader.Find("Hidden/MegaWorld/AreaMaskCrop"));
            }

            RenderTexture croppedRT = RenderTexture.GetTemporary(
                heightContext.sourceRenderTexture.width,
                heightContext.sourceRenderTexture.height,
                0,
                RenderTextureFormat.ARGB32);
            croppedRT.wrapMode = TextureWrapMode.Clamp;
            croppedRT.filterMode = FilterMode.Bilinear;

            _cropMaterial.SetTexture("_AreaMaskTex", sourceMask);
            ImageMaskUvTransform.SetAreaMaskCropMaterialProperties(_cropMaterial, centerUV, sizeUV);
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(heightContext, brushTransform, _cropMaterial);

            Graphics.Blit(Texture2D.blackTexture, croppedRT, _cropMaterial, 0);
            return croppedRT;
        }

        public static Texture2D CropCPU(Texture2D sourceMask, BoxArea fullArea, BoxArea stampArea, int cacheIndex)
        {
            int size = sourceMask.width;

            EnsureCPUCache(Mathf.Max(4, cacheIndex + 1), size);

            GetCropUVWindow(fullArea, stampArea, out Vector2 centerUV, out Vector2 sizeUV);

            float uMin = centerUV.x - sizeUV.x * 0.5f;
            float vMin = centerUV.y - sizeUV.y * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = uMin + (x / (float)(size - 1)) * sizeUV.x;
                    float v = vMin + (y / (float)(size - 1)) * sizeUV.y;
                    _cpuPixelBuffer[y * size + x] = sourceMask.GetPixelBilinear(u, v);
                }
            }

            _cpuCache[cacheIndex].SetPixels(_cpuPixelBuffer);
            _cpuCache[cacheIndex].Apply();

            return _cpuCache[cacheIndex];
        }

        private static void EnsureCPUCache(int count, int size)
        {
            if (_cpuCache != null &&
                _cpuCache.Length >= count &&
                _cpuCache[0] != null &&
                _cpuCache[0].width == size)
            {
                return;
            }

            _cpuPixelBuffer = new Color[size * size];
            _cpuCache = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                _cpuCache[i] = new Texture2D(size, size, TextureFormat.RGBA32, false);
            }
        }
    }
}
