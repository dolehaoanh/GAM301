using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour
{
    public class ComponentStack : NodeStackOnlyDifferentTypes<Node>
    {
        private UnityEngine.MonoBehaviour _tool;

        protected sealed override void OnSetup()
        {
            _tool = (UnityEngine.MonoBehaviour)SetupData[0];
        }

        protected sealed override void OnCreateElements()
        {
            AddMonoBehaviourComponentsAttribute addMonoBehaviourComponentsAttribute =
                (AddMonoBehaviourComponentsAttribute)_tool.GetType()
                    .GetAttribute(typeof(AddMonoBehaviourComponentsAttribute));

            if (addMonoBehaviourComponentsAttribute == null)
            {
                return;
            }

            CreateIfMissingType(addMonoBehaviourComponentsAttribute.Types);
        }

        protected sealed override void OnRemoveInvalidElementsAfterUniqueTypeCheck()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                AddMonoBehaviourComponentsAttribute addMonoBehaviourComponentsAttribute =
                    (AddMonoBehaviourComponentsAttribute)_tool.GetType()
                        .GetAttribute(typeof(AddMonoBehaviourComponentsAttribute));

                if (addMonoBehaviourComponentsAttribute == null)
                {
                    Remove(i);
                    continue;
                }

                if (!addMonoBehaviourComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}
