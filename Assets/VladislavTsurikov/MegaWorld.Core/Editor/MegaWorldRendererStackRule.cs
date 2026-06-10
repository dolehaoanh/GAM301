#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.MegaWorld.Editor
{
    public sealed class MegaWorldRendererStackRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol()
        {
            return "RENDERER_STACK";
        }

        public override string GetTypeFullName()
        {
            return "VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.TerrainObjectRenderer";
        }
    }
}
#endif
