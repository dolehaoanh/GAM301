#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    public static class AdvancedObjectBrushVisualisation
    {
        private static readonly MaskFilterVisualisation _maskFilterVisualisation = new();

        public static void Draw(BoxArea area, SelectionData data, LayerSettings layerSettings)
        {
            if (area == null || area.RayHit == null)
            {
                return;
            }

            if (data.SelectedData.HasOneSelectedGroup())
            {
                Group group = data.SelectedData.SelectedGroup;

                FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

                if (filterSettings.FilterType != FilterType.MaskFilter)
                {
                    SimpleFilterVisualisation.DrawSimpleFilter(group, area, filterSettings.SimpleFilter,
                        layerSettings);
                    VisualisationUtility.DrawCircleHandles(area);
                }
                else
                {
                    _maskFilterVisualisation.DrawMaskFilterVisualization(
                        filterSettings.MaskFilterComponentSettings.MaskFilterStack, area);
                }
            }
            else
            {
                if (SimpleFilterUtility.HasOneActiveSimpleFilter(typeof(AdvancedObjectBrush),
                        WindowData.Instance.SelectedData))
                {
                    VisualisationUtility.DrawCircleHandles(area);
                }
                else
                {
                    DrawShaderVisualisationUtility.DrawAreaPreview(area);
                }
            }
        }
    }
}
#endif
