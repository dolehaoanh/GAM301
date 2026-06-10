using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.BVH.Runtime
{
    public abstract class BVHNode<TData>
        where TData : class
    {
        private readonly List<BVHNode<TData>> _children = new();

        public TData Data { get; }

        public bool IsLeaf { get; private set; }

        public abstract Vector3 Position { get; set; }
        public abstract Vector3 Size { get; set; }
        public BVHNode<TData> Parent { get; private set; }

        public int NumChildren => _children.Count;

        public BVHNode()
        {
        }

        public BVHNode(TData data) => Data = data;

        public BVHNode<TData> GetChild(int index)
        {
            return _children[index];
        }

        public void MakeLeaf()
        {
            IsLeaf = true;
        }

        public void SetParent(BVHNode<TData> parent)
        {
            if (Parent != null)
            {
                Parent._children.Remove(this);
            }

            Parent = parent;
            if (Parent != null)
            {
                Parent._children.Add(this);
            }
        }

        public BVHNode<TData> FindClosestChild(BVHNode<TData> node)
        {
            BVHNode<TData> closestChild = null;
            float minDistSqr = float.MaxValue;
            foreach (BVHNode<TData> child in _children)
            {
                float distSqr = (node.Position - child.Position).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    closestChild = child;
                }
            }

            return closestChild;
        }

        public void EncapsulateChildrenBottomUp()
        {
            if (IsLeaf || NumChildren == 0)
            {
                return;
            }

            BVHNode<TData> currentParent = this;
            while (currentParent != null)
            {
                currentParent.Position = currentParent._children[0].Position;
                currentParent.Size = currentParent._children[0].Size;

                for (int childIndex = 1; childIndex < currentParent.NumChildren; ++childIndex)
                {
                    currentParent.EncapsulateNode(currentParent._children[childIndex]);
                }

                currentParent = currentParent.Parent;
            }
        }

        public abstract bool Raycast(Ray ray, out float t);
        public abstract bool IntersectsBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation);
        public abstract bool IntersectsSphere(Vector3 sphereCenter, float sphereRadius);

        protected abstract void EncapsulateNode(BVHNode<TData> node);
    }
}
