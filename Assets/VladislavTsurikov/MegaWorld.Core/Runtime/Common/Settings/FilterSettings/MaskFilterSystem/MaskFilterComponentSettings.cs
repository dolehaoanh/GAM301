using System;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Name("Mask Filter Settings")]
    [MegaWorldDocUrl("mask-filters/overview")]
    public class MaskFilterComponentSettings : Node
    {
        [NonSerialized]
        public MaskFilterContext FilterContext;

        [NonSerialized]
        public Texture2D FilterMaskTexture2D;

        [OdinSerialize]
        public MaskFilterStack MaskFilterStack = new();
    }
}
