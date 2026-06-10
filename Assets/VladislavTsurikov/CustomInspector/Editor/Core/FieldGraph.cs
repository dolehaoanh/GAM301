#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.Nody.Runtime.Core.Ports;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public sealed class FieldGraph
    {
        private readonly List<InspectorNodeConnection> _connections = new();
        private readonly List<InspectorNode> _nodes = new();

        public IReadOnlyList<InspectorNode> Nodes => _nodes;
        public IReadOnlyList<InspectorNodeConnection> Connections => _connections;

        public void AddNode(InspectorNode node)
        {
            if (node == null || _nodes.Contains(node))
            {
                return;
            }

            _nodes.Add(node);
        }

        public void Connect(InspectorNode fromNode, InspectorNode toNode)
        {
            if (fromNode == null || toNode == null)
            {
                return;
            }

            AddNode(fromNode);
            AddNode(toNode);

            NodePort fromPort = GetPort(fromNode, InspectorNode.FlowOutPortName, PortDirection.Output);
            NodePort toPort = GetPort(toNode, InspectorNode.FlowInPortName, PortDirection.Input);

            _connections.Add(new InspectorNodeConnection(
                fromNode.NodeId,
                toNode.NodeId,
                fromPort?.UniqueId,
                toPort?.UniqueId));
        }

        public void Execute(InspectorNodeContext context)
        {
            if (context == null || _nodes.Count == 0)
            {
                return;
            }

            context.ResetFlow();

            Dictionary<string, InspectorNode> nodesById = _nodes.ToDictionary(node => node.NodeId, node => node);
            Dictionary<string, List<InspectorNodeConnection>> outgoing = _connections
                .GroupBy(connection => connection.FromNodeId)
                .ToDictionary(group => group.Key, group => group.ToList());
            HashSet<string> incoming = new(_connections.Select(connection => connection.ToNodeId));

            List<InspectorNode> startNodes = _nodes.Where(node => !incoming.Contains(node.NodeId)).ToList();
            foreach (InspectorNode startNode in startNodes)
            {
                ExecuteFrom(startNode, context, nodesById, outgoing, new HashSet<string>());
            }
        }

        private static void ExecuteFrom(
            InspectorNode node,
            InspectorNodeContext context,
            Dictionary<string, InspectorNode> nodesById,
            Dictionary<string, List<InspectorNodeConnection>> outgoing,
            HashSet<string> path)
        {
            if (context.FlowStopped || node == null)
            {
                return;
            }

            if (!path.Add(node.NodeId))
            {
                return;
            }

            node.Execute(context);

            if (!context.FlowStopped &&
                outgoing.TryGetValue(node.NodeId, out List<InspectorNodeConnection> nextConnections))
            {
                foreach (InspectorNodeConnection connection in nextConnections)
                {
                    if (nodesById.TryGetValue(connection.ToNodeId, out InspectorNode nextNode))
                    {
                        ExecuteFrom(nextNode, context, nodesById, outgoing, path);
                    }
                }
            }

            path.Remove(node.NodeId);
        }

        private static NodePort GetPort(InspectorNode node, string portName, PortDirection direction)
        {
            return node.GetPorts()
                .GetAllPorts()
                .FirstOrDefault(port => port.Direction == direction && port.Name == portName);
        }
    }

    public sealed class InspectorNodeConnection
    {
        public string FromNodeId { get; }
        public string ToNodeId { get; }
        public string FromPortId { get; }
        public string ToPortId { get; }

        public InspectorNodeConnection(string fromNodeId, string toNodeId, string fromPortId, string toPortId)
        {
            FromNodeId = fromNodeId;
            ToNodeId = toNodeId;
            FromPortId = fromPortId;
            ToPortId = toPortId;
        }
    }
}
#endif
