#if UNITY_EDITOR
#if UNITY_2021_2_OR_NEWER
#else
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine.Experimental.TerrainAPI;
#endif
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint
{
    public static class VisualisationUtility
    {
        public static void DrawCircleHandles(BoxArea area)
        {
            LayerMask layerMask = GlobalCommonComponentSingleton<LayerSettings>.Instance.PaintLayers;
            BrushVisualisation.Draw(area, layerMask);
        }
    }
}
#endif
