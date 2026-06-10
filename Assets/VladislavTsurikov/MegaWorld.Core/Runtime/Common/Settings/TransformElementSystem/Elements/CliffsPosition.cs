using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Cliffs Position")]
    [MegaWorldDocUrl("transform-components/cliffs-position")]
    public class CliffsPosition : TransformComponent
    {
        public float OffsetPosition = 1;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            Vector3 direction = new(normal.x, 0, normal.z);

            instance.Position += direction + new Vector3(OffsetPosition, 0, OffsetPosition);
        }
    }
}
