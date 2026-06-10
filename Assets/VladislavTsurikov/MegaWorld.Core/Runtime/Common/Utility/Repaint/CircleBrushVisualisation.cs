#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint
{
    public static class CircleBrushVisualisation
    {
        public static void Draw(BoxArea area, LayerMask layerMask)
        {
            float perimeterLength = Mathf.PI * 2f * area.Radius;
            int resolution = Mathf.Clamp(
                Mathf.CeilToInt(perimeterLength / BrushVisualisation.SegmentLength),
                BrushVisualisation.MinResolution, BrushVisualisation.MaxResolution);

            List<Vector3> points = new(resolution);

            for (int i = 0; i < resolution; i++)
            {
                float angle = 2f * Mathf.PI * i / resolution;
                Vector3 surfacePoint = area.Center
                                       + area.Tangent * (Mathf.Cos(angle) * area.Radius)
                                       + area.Bitangent * (Mathf.Sin(angle) * area.Radius);

                Vector3 rayOrigin = surfacePoint + area.Normal * BrushVisualisation.RayStartOffset;

                if (Physics.Raycast(new Ray(rayOrigin, -area.Normal), out RaycastHit rayHit,
                        BrushVisualisation.RayDistance + BrushVisualisation.RayStartOffset, layerMask))
                {
                    points.Add(area.GetClosestPoint(rayHit.point));
                }
                else
                {
                    points.Add(surfacePoint);
                }
            }

            BrushVisualisation.DrawLoop(points, BrushVisualisation.GetBrushColor());
        }
    }
}
#endif
