#if UNITY_EDITOR
using System;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [Serializable]
    [Name("Object Brush Tool Settings")]
    public class AdvancedObjectBrushSettings : Node
    {
        public bool EnableFailureRateOnMouseDrag = true;
        public float FailureRateOnMouseDrag = 50f;
    }
}
#endif
