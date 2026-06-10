#if UNITY_EDITOR
using System.Collections.Generic;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public sealed class InspectorNodeSequence : InspectorNode
    {
        private readonly List<InspectorNode> _nodes = new();

        public InspectorNodeSequence()
        {
        }

        public InspectorNodeSequence(IEnumerable<InspectorNode> nodes)
        {
            if (nodes == null)
            {
                return;
            }

            _nodes.AddRange(nodes);
        }

        public void Add(InspectorNode node)
        {
            if (node == null)
            {
                return;
            }

            _nodes.Add(node);
        }

        public override void Execute(InspectorNodeContext context)
        {
            foreach (InspectorNode node in _nodes)
            {
                node.Execute(context);
            }
        }
    }
}
#endif
