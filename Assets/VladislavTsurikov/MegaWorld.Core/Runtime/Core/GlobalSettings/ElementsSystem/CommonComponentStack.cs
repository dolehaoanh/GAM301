using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem
{
    [Serializable]
    public class CommonComponentStack : NodeStackOnlyDifferentTypes<Node>
    {
        protected sealed override void OnCreateElements()
        {
            foreach (Type type in AllToolTypes.TypeList)
            {
                AddGlobalCommonComponentsAttribute addGlobalCommonComponentsAttribute =
                    type.GetAttribute<AddGlobalCommonComponentsAttribute>();

                if (addGlobalCommonComponentsAttribute == null)
                {
                    continue;
                }

                foreach (Type globalSettingsType in addGlobalCommonComponentsAttribute.Types)
                {
                    CreateIfMissingType(globalSettingsType);
                }
            }
        }

        protected sealed override void OnRemoveInvalidElementsAfterUniqueTypeCheck()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                bool find = false;

                foreach (Type type in AllToolTypes.TypeList)
                {
                    AddGlobalCommonComponentsAttribute attribute =
                        (AddGlobalCommonComponentsAttribute)type.GetAttribute(
                            typeof(AddGlobalCommonComponentsAttribute));

                    if (attribute == null)
                    {
                        continue;
                    }

                    if (attribute.Types.Contains(ElementList[i].GetType()))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    Remove(i);
                }
            }
        }
    }
}
