#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint
{
    public static class SquareBrushVisualisation
    {
        public static void Draw(BoxArea area, LayerMask layerMask)
        {
            if (!TryGetFaceCorners(area, out List<Vector3> orderedCorners))
            {
                return;
            }

            float rayDistance = GetRayDistanceFromObb(area, orderedCorners);
            List<Vector3> points = BuildProjectedFaceLoop(area, orderedCorners, rayDistance, layerMask);
            BrushVisualisation.DrawLoop(points, BrushVisualisation.GetBrushColor());
        }

        private static List<Vector3> BuildProjectedFaceLoop(BoxArea area, List<Vector3> orderedCorners,
            float rayDistance, LayerMask layerMask)
        {
            List<Vector3> result = new();

            for (int edgeIndex = 0; edgeIndex < orderedCorners.Count; edgeIndex++)
            {
                Vector3 start = orderedCorners[edgeIndex];
                Vector3 end = orderedCorners[(edgeIndex + 1) % orderedCorners.Count];
                float edgeLength = Vector3.Distance(start, end);
                int segmentCount = Mathf.Max(1, Mathf.CeilToInt(edgeLength / BrushVisualisation.SegmentLength));

                for (int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
                {
                    float t = (float)segmentIndex / segmentCount;
                    Vector3 pointOnFace = Vector3.Lerp(start, end, t);

                    Vector3 rayDirection = -area.Normal;

                    if (Physics.Raycast(new Ray(pointOnFace, rayDirection), out RaycastHit rayHit, rayDistance,
                            layerMask))
                    {
                        result.Add(rayHit.point);
                    }
                }
            }

            return result;
        }

        private static bool TryGetFaceCorners(BoxArea area, out List<Vector3> orderedCorners)
        {
            List<Vector3> corners = area.OBB.GetCornerPoints();
            Vector3 normal = area.Normal.normalized;
            Vector3 center = area.Center;

            float maxDot = float.MinValue;
            for (int i = 0; i < corners.Count; i++)
            {
                float dot = Vector3.Dot(corners[i] - center, normal);
                if (dot > maxDot)
                {
                    maxDot = dot;
                }
            }

            float tolerance = 0.001f;
            List<Vector3> faceCorners = new(4);
            for (int i = 0; i < corners.Count; i++)
            {
                float dot = Vector3.Dot(corners[i] - center, normal);
                if (Mathf.Abs(dot - maxDot) <= tolerance)
                {
                    faceCorners.Add(corners[i]);
                }
            }

            if (faceCorners.Count != 4)
            {
                faceCorners = corners
                    .OrderByDescending(point => Vector3.Dot(point - center, normal))
                    .Take(4)
                    .ToList();
            }

            if (faceCorners.Count != 4)
            {
                orderedCorners = null;
                return false;
            }

            Vector3 faceCenter = Vector3.zero;
            for (int i = 0; i < faceCorners.Count; i++)
            {
                faceCenter += faceCorners[i];
            }

            faceCenter /= faceCorners.Count;

            Vector3 axisX = (faceCorners[0] - faceCenter).normalized;
            if (axisX.sqrMagnitude < 0.0001f)
            {
                axisX = area.Tangent;
            }

            Vector3 axisY = Vector3.Cross(normal, axisX).normalized;
            if (axisY.sqrMagnitude < 0.0001f)
            {
                axisY = area.Bitangent;
            }

            orderedCorners = faceCorners
                .OrderBy(point =>
                {
                    Vector3 direction = point - faceCenter;
                    return Mathf.Atan2(Vector3.Dot(direction, axisY), Vector3.Dot(direction, axisX));
                })
                .ToList();

            return true;
        }

        private static float GetRayDistanceFromObb(BoxArea area, List<Vector3> faceCorners)
        {
            Vector3 normal = area.Normal.normalized;
            List<Vector3> allCorners = area.OBB.GetCornerPoints();

            float minProjection = float.MaxValue;
            float maxProjection = float.MinValue;

            for (int i = 0; i < allCorners.Count; i++)
            {
                float projection = Vector3.Dot(allCorners[i], normal);
                if (projection < minProjection)
                {
                    minProjection = projection;
                }

                if (projection > maxProjection)
                {
                    maxProjection = projection;
                }
            }

            return Mathf.Max(maxProjection - minProjection, BrushVisualisation.SegmentLength);
        }
    }
}
#endif
