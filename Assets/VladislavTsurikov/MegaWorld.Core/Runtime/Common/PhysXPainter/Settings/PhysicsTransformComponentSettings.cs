using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings
{
    [Name("Physics Transform Settings")]
    [MegaWorldDocUrl("shared-settings/physics-transform-settings")]
    public class PhysicsTransformComponentSettings : Node
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
