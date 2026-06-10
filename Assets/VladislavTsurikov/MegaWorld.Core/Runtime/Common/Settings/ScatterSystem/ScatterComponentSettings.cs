using System;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    [Name("Scatter Settings")]
    [MegaWorldDocUrl("scatter-system/scatter-settings")]
    public class ScatterComponentSettings : Node
    {
        public ScatterStack ScatterStack = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            ScatterStack.Setup();
        }

        protected override void OnCreate()
        {
            ScatterStack.CreateIfMissingType(typeof(RandomGrid));
        }
    }
}
