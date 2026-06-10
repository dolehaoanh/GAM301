using System;
using System.Linq;
using OdinSerializer;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    public class GeneralComponentStack : NodeStackOnlyDifferentTypes<Node>
    {
        [OdinSerialize]
        private Type _addElementsAttributeType;

        [OdinSerialize]
        private Type _prototypeType;

        protected sealed override void OnSetup()
        {
            _addElementsAttributeType = (Type)SetupData[0];
            _prototypeType = (Type)SetupData[1];
        }

        protected sealed override void OnCreateElements()
        {
            foreach (Type type in AllToolTypes.TypeList)
            foreach (Attribute attribute in type.GetAttributes(_addElementsAttributeType))
            {
                AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                {
                    continue;
                }

                CreateIfMissingType(addComponentsAttribute.Types);
            }
        }

        protected sealed override void OnRemoveInvalidElementsAfterUniqueTypeCheck()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                bool found = false;

                foreach (Type toolType in AllToolTypes.TypeList)
                {
                    if (IsElementBelongsToTool(ElementList[i], toolType))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Remove(i);
                }
            }
        }

        private bool IsElementBelongsToTool(Element element, Type toolType)
        {
            foreach (Attribute attribute in toolType.GetAttributes(_addElementsAttributeType))
            {
                AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                {
                    continue;
                }

                if (addComponentsAttribute.Types.Contains(element.GetType()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
