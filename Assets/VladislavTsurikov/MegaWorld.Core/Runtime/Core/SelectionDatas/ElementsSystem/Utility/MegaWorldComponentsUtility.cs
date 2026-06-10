using System;
using System.Collections;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Utility
{
    public static class MegaWorldComponentsUtility
    {
        public static IEnumerable GetAttributes(Type addElementsAttributeType, Type prototypeType, Type toolType)
        {
            if (addElementsAttributeType == null || toolType == null || prototypeType == null)
            {
                yield break;
            }

            foreach (Attribute attribute1 in toolType.GetAttributes(addElementsAttributeType))
            {
                AddComponentsAttribute attribute = (AddComponentsAttribute)attribute1;
                if (attribute != null &&
                    attribute.PrototypeTypes != null &&
                    attribute.PrototypeTypes.Contains(prototypeType))
                {
                    yield return attribute;
                }
            }
        }
    }
}
