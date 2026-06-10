using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Scale Fitness")]
    [MegaWorldDocUrl("transform-components/scale-fitness")]
    public class ScaleFitness : TransformComponent
    {
        public float OffsetScale = -0.7f;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            float value = Mathf.Lerp(OffsetScale, 0, fitness);

            instance.Scale += new Vector3(value, value, value);
        }
    }
}
