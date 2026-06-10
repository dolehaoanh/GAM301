using System;
using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.TerrainUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Area Mask")]
    [MegaWorldDocUrl("mask-filters/area-mask-filter")]
    public class AreaMaskFilter : MaskFilter
    {
        [SerializeField]
        private string _maskId = string.Empty;

        public BlendMode BlendMode = BlendMode.Multiply;
        public ImageMaskOrientation Orientation;
        private Material _cropMaterial;

        private Material _imageMaterial;
        private Material _rotateMaterial;

        public string MaskId
        {
            get => _maskId;
            set => _maskId = value ?? string.Empty;
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            Texture2D areaMaskTexture = ResolveMaskTexture(maskFilterContext);
            if (areaMaskTexture == null)
            {
                Graphics.Blit(
                    string.IsNullOrWhiteSpace(MaskId) ? maskFilterContext.SourceRenderTexture : Texture2D.blackTexture,
                    maskFilterContext.DestinationRenderTexture);
                return;
            }

            RenderTexture rotatedMask = null;
            RenderTexture croppedMask = null;
            Material material = GetImageMaterial();
            try
            {
                rotatedMask = CreateRotatedMask(areaMaskTexture);
                Texture maskForCrop = rotatedMask != null ? rotatedMask : areaMaskTexture;
                croppedMask = CreateCroppedMask(maskFilterContext, maskForCrop);

                material.SetTexture("_InputTex", maskFilterContext.SourceRenderTexture);
                material.SetInt("_BlendMode", index == 0 ? (int)BlendMode.Multiply : (int)BlendMode);
                material.SetTexture("_ImageMaskTex", croppedMask != null ? croppedMask : maskForCrop);
                material.SetInt("_FilterMode", (int)FilterMode.GrayScale);
                material.SetColor("_Color", Color.white);
                material.SetFloat("_ColorAccuracy", 1f);
                material.SetFloat("_XOffset", 0f);
                material.SetFloat("_ZOffset", 0f);

                Graphics.Blit(maskFilterContext.SourceRenderTexture, maskFilterContext.DestinationRenderTexture,
                    material, 0);
            }
            finally
            {
                if (croppedMask != null)
                {
                    RenderTexture.ReleaseTemporary(croppedMask);
                }

                if (rotatedMask != null)
                {
                    RenderTexture.ReleaseTemporary(rotatedMask);
                }
            }
        }

        private Material GetImageMaterial()
        {
            if (_imageMaterial == null)
            {
                _imageMaterial = new Material(Shader.Find("Hidden/MegaWorld/Image"));
            }

            return _imageMaterial;
        }

        private Material GetCropMaterial()
        {
            if (_cropMaterial == null)
            {
                _cropMaterial = new Material(Shader.Find("Hidden/MegaWorld/AreaMaskCrop"));
            }

            return _cropMaterial;
        }

        private Material GetRotateMaterial()
        {
            if (_rotateMaterial == null)
            {
                _rotateMaterial = new Material(Shader.Find("Hidden/MegaWorld/AreaMaskRotate"));
            }

            return _rotateMaterial;
        }

        private Texture2D ResolveMaskTexture(MaskFilterContext maskFilterContext)
        {
            if (string.IsNullOrWhiteSpace(MaskId))
            {
                return null;
            }

            Terrain terrain = maskFilterContext?.BoxArea?.TerrainUnder;
            if (terrain == null)
            {
                return null;
            }

            AreaMask areaMask = terrain.GetComponent<AreaMask>();
            if (areaMask == null)
            {
                return null;
            }

            return areaMask.TryGetMask(MaskId, out Texture2D maskTexture) ? maskTexture : null;
        }

        private RenderTexture CreateRotatedMask(Texture2D areaMaskTexture)
        {
            if (Orientation == ImageMaskOrientation.None)
            {
                return null;
            }

            RenderTexture rotatedMask = RenderTexture.GetTemporary(areaMaskTexture.width, areaMaskTexture.height, 0,
                RenderTextureFormat.ARGB32);
            rotatedMask.wrapMode = TextureWrapMode.Clamp;
            rotatedMask.filterMode = UnityEngine.FilterMode.Bilinear;

            Material rotateMaterial = GetRotateMaterial();
            rotateMaterial.SetTexture("_AreaMaskTex", areaMaskTexture);
            ImageMaskUvTransform.SetAreaMaskRotateMaterialProperties(rotateMaterial, Orientation);

            Graphics.Blit(Texture2D.blackTexture, rotatedMask, rotateMaterial, 0);
            return rotatedMask;
        }

        private RenderTexture CreateCroppedMask(MaskFilterContext maskFilterContext, Texture areaMaskTexture)
        {
            if (maskFilterContext?.SourceRenderTexture == null ||
                maskFilterContext.HeightContext == null ||
                !TryGetMaskUvWindow(maskFilterContext, out Vector2 centerUv, out Vector2 sizeUv))
            {
                return null;
            }

            RenderTexture source = maskFilterContext.SourceRenderTexture;
            RenderTexture croppedMask =
                RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            croppedMask.wrapMode = TextureWrapMode.Clamp;
            croppedMask.filterMode = UnityEngine.FilterMode.Bilinear;

            Material cropMaterial = GetCropMaterial();
            cropMaterial.SetTexture("_AreaMaskTex", areaMaskTexture);
            ImageMaskUvTransform.SetAreaMaskCropMaterialProperties(cropMaterial, centerUv, sizeUv);
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(maskFilterContext.HeightContext,
                maskFilterContext.BrushTransform, cropMaterial);

            Graphics.Blit(Texture2D.blackTexture, croppedMask, cropMaterial, 0);
            return croppedMask;
        }

        private static bool TryGetMaskUvWindow(MaskFilterContext maskFilterContext, out Vector2 centerUv,
            out Vector2 sizeUv)
        {
            centerUv = new Vector2(0.5f, 0.5f);
            sizeUv = Vector2.one;

            if (maskFilterContext?.BoxArea == null)
            {
                return false;
            }

            Bounds terrainBounds = maskFilterContext.BoxArea.TerrainBounds;
            if (terrainBounds.size.x <= 0f || terrainBounds.size.z <= 0f)
            {
                return false;
            }

            BoxArea boxArea = maskFilterContext.BoxArea;
            float centerU = Mathf.InverseLerp(terrainBounds.min.x, terrainBounds.max.x, boxArea.Center.x);
            float centerV = Mathf.InverseLerp(terrainBounds.min.z, terrainBounds.max.z, boxArea.Center.z);
            float sizeU = boxArea.LocalSize.x / terrainBounds.size.x;
            float sizeV = boxArea.LocalSize.z / terrainBounds.size.z;

            if (sizeU <= 0f || sizeV <= 0f)
            {
                return false;
            }

            centerUv = new Vector2(Mathf.Clamp01(centerU), Mathf.Clamp01(centerV));
            sizeUv = new Vector2(Mathf.Clamp01(sizeU), Mathf.Clamp01(sizeV));
            return true;
        }
    }
}
