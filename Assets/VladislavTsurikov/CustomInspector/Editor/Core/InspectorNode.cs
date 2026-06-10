#if UNITY_EDITOR
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core.Ports;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class InspectorNode : Node
    {
        public const string FlowInPortName = "FlowIn";
        public const string FlowOutPortName = "FlowOut";

        public override void OnDefinePorts(PortDefinitionContext context)
        {
            context.AddInputPort<bool>(FlowInPortName);
            context.AddOutputPort<bool>(FlowOutPortName);
            DefinePorts(context);
        }

        protected virtual void DefinePorts(PortDefinitionContext context)
        {
        }

        public abstract void Execute(InspectorNodeContext context);
    }
}
#endif
