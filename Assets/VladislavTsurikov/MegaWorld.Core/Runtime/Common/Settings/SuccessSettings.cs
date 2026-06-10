using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Name("Success")]
    [MegaWorldDocUrl("shared-settings/success")]
    public class SuccessSettings : Node
    {
        [Name("Success (%)")]
        [Range(0f, 100f)]
        public float SuccessValue = 100f;
    }
}
