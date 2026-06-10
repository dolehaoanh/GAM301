#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.MegaWorld.Editor
{
    public sealed class PhysicsSimulatorRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol()
        {
            return "PHYSICS_SIMULATOR";
        }

        public override string GetTypeFullName()
        {
            return "VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody";
        }
    }
}
#endif
