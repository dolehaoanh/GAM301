using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Cliffs Align")]
    [MegaWorldDocUrl("transform-components/cliffs-align")]
    public class CliffsAlign : TransformComponent
    {
        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            Vector3 direction = new(normal.x, 0, normal.z);

            float distancePositive = Vector3.Distance(Vector3.right, direction);
            float distanceNegative = Vector3.Distance(-Vector3.right, direction);

            if (distancePositive < distanceNegative)
            {
                float angle = Vector3.Angle(Vector3.forward, direction);
                instance.Rotation = Quaternion.AngleAxis(angle, Vector3.up) * instance.Rotation;
            }
            else
            {
                float angle = -Vector3.Angle(Vector3.forward, direction);
                instance.Rotation = Quaternion.AngleAxis(angle, Vector3.up) * instance.Rotation;
            }
        }
    }
}
