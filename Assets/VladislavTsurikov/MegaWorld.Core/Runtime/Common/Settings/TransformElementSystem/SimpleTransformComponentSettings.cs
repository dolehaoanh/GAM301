using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Name("Simple Transform Settings")]
    [MegaWorldDocUrl("shared-settings/simple-transform-settings")]
    public class SimpleTransformComponentSettings : Node
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
