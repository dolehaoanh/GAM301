#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;

namespace VladislavTsurikov.MegaWorld.Editor.SprayBrushTool
{
    public static class SprayBrushToolVisualisation
    {
        public static void Draw(BoxArea boxArea)
        {
            if (boxArea == null || boxArea.RayHit == null)
            {
                return;
            }

            if (WindowData.Instance.SelectedData.HasOneSelectedGroup())
            {
                Group group = WindowData.Instance.SelectedData.SelectedGroup;

                SimpleFilter simpleFilter =
                    (SimpleFilter)group.GetElement(typeof(SprayBrushTool), typeof(SimpleFilter));

                if (simpleFilter.HasOneActiveFilter())
                {
                    SimpleFilterVisualisation.DrawSimpleFilter(group, boxArea, simpleFilter,
                        GlobalCommonComponentSingleton<LayerSettings>.Instance, true);
                }

                VisualisationUtility.DrawCircleHandles(boxArea);
            }
            else
            {
                VisualisationUtility.DrawCircleHandles(boxArea);
            }
        }
    }
}
#endif
