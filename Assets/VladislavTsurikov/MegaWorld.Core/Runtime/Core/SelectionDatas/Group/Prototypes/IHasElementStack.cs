using System;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    public interface IHasElementStack
    {
        ComponentStackManager ComponentStackManager { get; }

        public void SetupComponentStack();

        public Node GetElement(Type elementType)
        {
            return ComponentStackManager.GeneralComponentStack.GetElement(elementType);
        }

        public Node GetElement(Type toolType, Type elementType)
        {
            return ComponentStackManager.ToolsComponentStack.GetElement(elementType, toolType);
        }
    }
}
