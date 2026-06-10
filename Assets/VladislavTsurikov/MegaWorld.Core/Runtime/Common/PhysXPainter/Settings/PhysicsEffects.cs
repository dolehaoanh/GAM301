using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
#if PHYSICS_SIMULATOR
using VladislavTsurikov.PhysicsSimulator.Runtime;
#endif
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings
{
    [Name("Physics Effects")]
    [MegaWorldDocUrl("shared-settings/physics-effects-settings")]
    public class PhysicsEffects : Node
    {
        #region Direction

        public float RandomStrength = 50f;

        #endregion

        public void ApplyForce(Rigidbody rigidbody)
        {
            if (rigidbody == null)
            {
                return;
            }

            float radians = Random.Range(0, 360) * Mathf.Deg2Rad;

            Vector3 forceDirection = new(Mathf.Sin(radians), 0, Mathf.Cos(radians));

            Vector3 force = Vector3.Lerp(new Vector3(0, -1, 0), forceDirection, RandomStrength / 100);

            float magnitude = ForceRange ? Random.Range(MinForce, MaxForce) : MinForce;

            force *= magnitude;

#if PHYSICS_SIMULATOR
            PhysicsUtility.ApplyForce(rigidbody, force);
#else
            rigidbody.AddForce(force, ForceMode.Impulse);
#endif
        }

        #region Force

        public bool ForceRange = true;
        public float MinForce = 10f;
        public float MaxForce = 40f;

        #endregion
    }
}
