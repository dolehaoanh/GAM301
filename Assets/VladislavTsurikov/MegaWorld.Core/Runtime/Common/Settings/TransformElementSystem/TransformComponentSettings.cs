using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Name("Transform Settings")]
    [MegaWorldDocUrl("shared-settings/physics-transform-settings")]
    public class TransformComponentSettings : Node
    {
        public TransformComponentStack TransformComponentStack = new();

        protected override void OnCreate()
        {
            TransformComponentStack.CreateIfMissingType(typeof(PositionOffset));
            TransformComponentStack.CreateIfMissingType(typeof(Rotation));
            TransformComponentStack.CreateIfMissingType(typeof(Scale));
        }
    }
}
