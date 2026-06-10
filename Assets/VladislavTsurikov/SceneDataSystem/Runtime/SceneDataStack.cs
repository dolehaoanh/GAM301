using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    public class SceneDataStack : NodeStackOnlyDifferentTypes<SceneData>
    {
        protected sealed override bool AllowCreateNodeStack(Type type)
        {
            AllowCreateComponentAttribute allowCreateComponentAttribute =
                type.GetAttribute<AllowCreateComponentAttribute>();

            if (allowCreateComponentAttribute == null ||
                allowCreateComponentAttribute.Allow((SceneDataManager)SetupData[0]))
            {
                return true;
            }

            return false;
        }
    }
}
