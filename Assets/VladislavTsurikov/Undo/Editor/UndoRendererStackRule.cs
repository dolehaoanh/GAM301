#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.Undo.Editor
{
    public sealed class UndoRendererStackRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol()
        {
            return "UNDO_RENDERER_STACK";
        }

        public override string GetTypeFullName()
        {
            return "VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.TerrainObjectRenderer";
        }
    }
}
#endif
