#if UNITY_EDITOR
namespace VladislavTsurikov.Popup.Editor.Selector
{
    public sealed class SelectorItemNode : SelectorNode
    {
        public string Path { get; set; }
        public bool ItemIsEnabled { get; set; } = true;
        public object SourceObject { get; set; }

        public override bool IsEnabled => ItemIsEnabled;
    }
}
#endif
