using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Area
{
    public class Area
    {
        public Texture Mask = Texture2D.whiteTexture;

        public float Rotation = 0;
        public Shape Shape = Shape.Square;

        public float SizeMultiplier
        {
            get
            {
                if (Rotation == 0)
                {
                    return 1;
                }

                float sizeMultiplier = Mathf.Abs(CosAngle);
                sizeMultiplier += Mathf.Abs(SinAngle);

                return sizeMultiplier;
            }
        }

        public Vector3 Size
        {
            get => Shape == Shape.Circle ? LocalSize : LocalSize * SizeMultiplier;
            protected set => LocalSize = value;
        }

        public Vector3 LocalSize { get; protected set; }

        public float CosAngle => Mathf.Cos(Rotation * Mathf.Deg2Rad);
        public float SinAngle => Mathf.Sin(Rotation * Mathf.Deg2Rad);

        public Vector3 Center { get; }

        public float Radius => LocalSize.x * 0.5f;

        public virtual Quaternion RotationQuaternion => Quaternion.Euler(0f, Rotation, 0f);

        public virtual OBB OBB => new(Center, LocalSize, RotationQuaternion);

        public virtual Sphere Sphere => new(Center, Radius);

        public virtual AABB AABB => Shape == Shape.Circle
            ? new AABB(Center, Vector3.one * (Radius * 2f))
            : new AABB(OBB.GetCornerPoints());

        public Bounds Bounds => AABB.ToBounds();

        protected Area(Vector3 center, Vector3 size)
        {
            Center = center;
            LocalSize = size;
        }

        protected Area(Vector3 center, float size) : this(center, new Vector3(size, size, size))
        {
        }

        public bool Contains(Vector3 point)
        {
            return Shape == Shape.Circle
                ? Sphere.Contains(point)
                : OBB.ContainsPoint(point);
        }

        public bool Contains(Vector2 point)
        {
            return Contains(new Vector3(point.x, Center.y, point.y));
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            return Shape == Shape.Circle
                ? Sphere.GetClosestPoint(point)
                : OBB.GetClosestPoint(point);
        }

        public float GetAlpha(Vector2 pos, Vector2 size)
        {
            Texture2D mask2D = Mask as Texture2D;
            if (mask2D == null)
            {
                return 1.0f;
            }

            pos += Vector2Int.one;

            if (Rotation == 0.0f)
            {
                return TextureUtility.GetAlpha(pos, size, mask2D);
            }

            Vector2 halfTarget = size / 2.0f;
            Vector2 origin = pos - halfTarget;
            origin *= SizeMultiplier;
            origin = new Vector2(
                origin.x * CosAngle - origin.y * SinAngle + halfTarget.x,
                origin.x * SinAngle + origin.y * CosAngle + halfTarget.y);

            if (origin.x < 0.0f || origin.x > size.x || origin.y < 0.0f || origin.y > size.y)
            {
                return 0.0f;
            }

            return TextureUtility.GetAlpha(origin, size, mask2D);
        }
    }
}
