using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public struct Sphere
    {
        public Vector3 Center;
        public float Radius;
        public bool IsValid { get; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
            IsValid = true;
        }

        public bool Contains(Vector3 point)
        {
            return Vector3.Distance(point, Center) <= Radius;
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 direction = point - Center;
            float distance = direction.magnitude;

            if (distance <= Radius || distance == 0f)
            {
                return point;
            }

            return Center + direction / distance * Radius;
        }

        public bool Intersects(Sphere otherSphere)
        {
            float distance = Vector3.Distance(Center, otherSphere.Center);

            if (distance < Radius + otherSphere.Radius)
            {
                return true;
            }

            return false;
        }

        public bool Intersects(OBB obb)
        {
            return obb.IntersectsSphere(Center, Radius);
        }

        public bool Intersects(AABB aabb)
        {
            return aabb.IntersectsSphere(Center, Radius);
        }
    }
}
