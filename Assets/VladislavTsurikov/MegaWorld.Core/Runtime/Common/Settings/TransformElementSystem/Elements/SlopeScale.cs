using System;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Slope Scale")]
    [MegaWorldDocUrl("transform-components/slope-scale")]
    public class SlopeScale : TransformComponent
    {
        public bool UniformScaleOffset = true;

        [Range(0.1f, 90f)]
        public float MaxSlope = 30;

        [ShowIf("UniformScaleOffset", true)]
        [UnityEngine.Min(0.1f)]
        public float MaxUniformScaleOffset = 2;

        [ShowIf("UniformScaleOffset", false)]
        public Vector3 MaxScaleOffset = new(2, 2, 0.5f);

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            float normalAngle = Vector3.Angle(normal, Vector3.up);
            float difference = normalAngle / MaxSlope;

            if (UniformScaleOffset)
            {
                float value = Mathf.Lerp(0, MaxUniformScaleOffset, difference);

                instance.Scale += new Vector3(value, value, value);
            }
            else
            {
                float valueX = Mathf.Lerp(0, MaxScaleOffset.x, difference);
                float valueY = Mathf.Lerp(0, MaxScaleOffset.y, difference);
                float valueZ = Mathf.Lerp(0, MaxScaleOffset.z, difference);

                instance.Scale += new Vector3(valueX, valueY, valueZ);
            }
        }
    }
}
