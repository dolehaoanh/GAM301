#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    public class StamperMaskFilterVisualisation : MaskFilterVisualisation
    {
        protected override bool IsNeedUpdateMask(MaskFilterStack maskFilterStack, BoxArea boxArea)
        {
            if (NeedUpdateMask)
            {
                return true;
            }

            return base.IsNeedUpdateMask(maskFilterStack, boxArea);
        }
    }
}
#endif
