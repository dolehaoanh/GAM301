#if UNITY_EDITOR
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    [DontDrawFoldout]
    [ElementEditor(typeof(Area))]
    public class AreaEditor : IMGUIElementEditor
    {
        public override void OnGUI()
        {
        }
    }
}
#endif
