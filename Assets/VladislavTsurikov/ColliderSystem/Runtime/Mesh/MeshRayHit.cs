using UnityEngine;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public class MeshRayHit
    {
        public float HitEnter { get; }

        public Vector3 HitPoint { get; }

        public Vector3 HitNormal { get; }

        public int TriangleIndex { get; }

        public MeshRayHit(Ray ray, float hitEnter, int triangleIndex, Vector3 hitNormal)
        {
            HitEnter = hitEnter;
            HitPoint = ray.GetPoint(hitEnter);
            TriangleIndex = triangleIndex;
            HitNormal = hitNormal;
        }
    }
}
