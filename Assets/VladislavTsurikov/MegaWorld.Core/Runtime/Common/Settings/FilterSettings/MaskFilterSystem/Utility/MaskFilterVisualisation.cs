#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public class MaskFilterVisualisation
    {
        protected MaskFilterContext _filterContext;

        public bool NeedUpdateMask = true;
        protected MaskFilterStack PastMaskFilterStack;

        protected virtual bool IsNeedUpdateMask(MaskFilterStack maskFilterStack, BoxArea boxArea)
        {
            if (NeedUpdateMask)
            {
                return true;
            }

            if (_filterContext == null)
            {
                return true;
            }

            if (PastMaskFilterStack != maskFilterStack)
            {
                return true;
            }

            return !AreAreasEqual(_filterContext.BoxArea, boxArea);
        }

        public void DrawMaskFilterVisualization(MaskFilterStack maskFilterStack, BoxArea area, float multiplyAlpha = 1)
        {
            if (area.TerrainUnder == null)
            {
                return;
            }

            if (maskFilterStack.ElementList.Count > 0)
            {
                if (IsNeedUpdateMask(maskFilterStack, area))
                {
                    UpdateMask(maskFilterStack, area);
                }

                DrawShaderVisualisationUtility.DrawMaskFilter(area, _filterContext, multiplyAlpha);
            }
            else
            {
                DrawShaderVisualisationUtility.DrawAreaPreview(area);
            }
        }

        private void UpdateMask(MaskFilterStack maskFilterStack, BoxArea boxArea)
        {
            if (_filterContext == null)
            {
                _filterContext = new MaskFilterContext(boxArea);
            }
            else
            {
                _filterContext.Dispose();
            }

            FilterMaskOperation.UpdateFilterContext(ref _filterContext, maskFilterStack, boxArea);

            PastMaskFilterStack = maskFilterStack;

            NeedUpdateMask = false;
        }

        private static bool AreAreasEqual(BoxArea left, BoxArea right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            if (left.TerrainUnder != right.TerrainUnder)
            {
                return false;
            }

            return Approximately(left.Center, right.Center) &&
                   Approximately(left.LocalSize, right.LocalSize) &&
                   Approximately(left.Bounds.center, right.Bounds.center) &&
                   Approximately(left.Bounds.size, right.Bounds.size) &&
                   Approximately(left.TerrainBounds.center, right.TerrainBounds.center) &&
                   Approximately(left.TerrainBounds.size, right.TerrainBounds.size);
        }

        private static bool Approximately(Vector3 left, Vector3 right)
        {
            return Mathf.Abs(left.x - right.x) < 0.001f &&
                   Mathf.Abs(left.y - right.y) < 0.001f &&
                   Mathf.Abs(left.z - right.z) < 0.001f;
        }
    }
}
#endif
