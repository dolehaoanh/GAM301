using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public static class ImageMaskUvTransform
    {
        public static void SetAreaMaskCropMaterialProperties(Material material, Vector2 centerUv, Vector2 sizeUv)
        {
            material.SetFloat("_AreaMaskCenterU", centerUv.x);
            material.SetFloat("_AreaMaskCenterV", centerUv.y);
            material.SetFloat("_AreaMaskSizeU", sizeUv.x);
            material.SetFloat("_AreaMaskSizeV", sizeUv.y);
        }

        public static void SetAreaMaskRotateMaterialProperties(Material material, ImageMaskOrientation orientation)
        {
            GetAxes(orientation, out Vector2 uAxis, out Vector2 vAxis);

            material.SetFloat("_AreaMaskUAxisX", uAxis.x);
            material.SetFloat("_AreaMaskUAxisY", uAxis.y);
            material.SetFloat("_AreaMaskVAxisX", vAxis.x);
            material.SetFloat("_AreaMaskVAxisY", vAxis.y);
        }

        private static void GetAxes(ImageMaskOrientation orientation, out Vector2 uAxis, out Vector2 vAxis)
        {
            switch (orientation)
            {
                case ImageMaskOrientation.FlipX:
                    uAxis = new Vector2(-1f, 0f);
                    vAxis = new Vector2(0f, 1f);
                    break;
                case ImageMaskOrientation.FlipZ:
                    uAxis = new Vector2(1f, 0f);
                    vAxis = new Vector2(0f, -1f);
                    break;
                case ImageMaskOrientation.Rotate90Clockwise:
                    uAxis = new Vector2(0f, -1f);
                    vAxis = new Vector2(1f, 0f);
                    break;
                case ImageMaskOrientation.Rotate180:
                    uAxis = new Vector2(-1f, 0f);
                    vAxis = new Vector2(0f, -1f);
                    break;
                case ImageMaskOrientation.Rotate90CounterClockwise:
                    uAxis = new Vector2(0f, 1f);
                    vAxis = new Vector2(-1f, 0f);
                    break;
                default:
                    uAxis = new Vector2(1f, 0f);
                    vAxis = new Vector2(0f, 1f);
                    break;
            }
        }
    }
}
