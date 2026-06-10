using System.Linq;
using OdinSerializer;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponentStack : NodeStackOnlyDifferentTypes<Node>
    {
        [OdinSerialize]
        private Group _group;

        protected sealed override void OnSetup()
        {
            _group = (Group)SetupData[0];
        }

        protected sealed override void OnCreateElements()
        {
            AddDefaultGroupComponentsAttribute addDefaultGroupComponentsAttribute =
                _group.PrototypeType.GetAttribute<AddDefaultGroupComponentsAttribute>();

            if (addDefaultGroupComponentsAttribute == null)
            {
                return;
            }

            CreateIfMissingType(addDefaultGroupComponentsAttribute.Types);
        }

        protected sealed override void OnRemoveInvalidElementsAfterUniqueTypeCheck()
        {
            AddDefaultGroupComponentsAttribute addDefaultGroupComponentsAttribute =
                _group.PrototypeType.GetAttribute<AddDefaultGroupComponentsAttribute>();

            if (addDefaultGroupComponentsAttribute == null)
            {
                RemoveAll();
                return;
            }

            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                if (!addDefaultGroupComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}
