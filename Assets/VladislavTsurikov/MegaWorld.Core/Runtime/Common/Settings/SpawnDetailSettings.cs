using System;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Serializable]
    [Name("Spawn Detail Settings")]
    [MegaWorldDocUrl("general-settings/spawn-detail-settings")]
    public class SpawnDetailSettings : Node
    {
        public bool UseRandomOpacity = true;
        public int Density = 5;
        public float FailureRate;
    }
}
