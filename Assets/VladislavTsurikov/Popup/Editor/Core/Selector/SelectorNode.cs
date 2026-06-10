#if UNITY_EDITOR
using VladislavTsurikov.GraphRuntime.Runtime;

namespace VladislavTsurikov.Popup.Editor.Selector
{
    public abstract class SelectorNode : Node<SelectorNode>
    {
        public string Name { get; set; }
        public int Depth { get; set; }
        public virtual bool IsEnabled => true;
    }
}
#endif
