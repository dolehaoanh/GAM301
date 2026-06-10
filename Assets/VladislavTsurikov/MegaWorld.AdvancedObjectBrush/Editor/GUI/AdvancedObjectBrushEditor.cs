#if UNITY_EDITOR
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [ElementEditor(typeof(AdvancedObjectBrush))]
    public class AdvancedObjectBrushEditor : ToolWindowEditor
    {
        public override void DrawButtons()
        {
            UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);
        }
    }
}
#endif
