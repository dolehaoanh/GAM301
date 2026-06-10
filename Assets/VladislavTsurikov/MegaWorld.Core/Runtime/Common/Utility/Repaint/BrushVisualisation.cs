#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.AdvancedBrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint
{
    public static class BrushVisualisation
    {
        internal const int MinResolution = 16;
        internal const int MaxResolution = 128;
        internal const float SegmentLength = 0.5f;
        internal const float RayStartOffset = 100.0f;
        internal const float RayDistance = 1000.0f;
        private const float LineWidthOuter = 8.0f;

        private const float LineWidthInner = 4.0f;
        //private const bool DebugMode = false;

        public static void Draw(BoxArea area, LayerMask layerMask)
        {
            if (area?.RayHit == null)
            {
                return;
            }

            switch (area.Shape)
            {
                case Shape.Circle:
                    CircleBrushVisualisation.Draw(area, layerMask);
                    // if (DebugMode)
                    // {
                    //     DrawDebugSphere(area);
                    // }

                    break;
                case Shape.Square:
                    SquareBrushVisualisation.Draw(area, layerMask);
                    // if (DebugMode)
                    // {
                    //     DrawDebugObb(area);
                    // }

                    break;
            }
        }

        internal static void DrawLoop(IReadOnlyList<Vector3> points, Color color)
        {
            if (points == null || points.Count == 0)
            {
                return;
            }

            Vector3[] arr = new Vector3[points.Count + 1];
            for (int i = 0; i < points.Count; i++)
            {
                arr[i] = points[i];
            }

            arr[points.Count] = points[0];

            CompareFunction oldZTest = Handles.zTest;
            Handles.zTest = CompareFunction.Always;

            Handles.color = new Color(0f, 0f, 0f, color.a);
            Handles.DrawAAPolyLine(LineWidthOuter, arr);

            Handles.color = color;
            Handles.DrawAAPolyLine(LineWidthInner, arr);

            Handles.zTest = oldZTest;
        }

        internal static Color GetBrushColor()
        {
            return PreferenceElementSingleton<VisualisationBrushHandlesPreference>.Instance.CircleColor;
        }

        /*private static void DrawDebugSphere(BoxArea area)
        {
            var sphere = area.Sphere;
            Color color = GetDebugColor();
            Color oldColor = Handles.color;

            Handles.color = color;
            Handles.DrawWireDisc(sphere.Center, area.Tangent, sphere.Radius);
            Handles.DrawWireDisc(sphere.Center, area.Bitangent, sphere.Radius);
            Handles.DrawWireDisc(sphere.Center, area.Normal, sphere.Radius);
            Handles.color = oldColor;
        }

        private static void DrawDebugObb(BoxArea area)
        {
            Color oldColor = Handles.color;
            Matrix4x4 oldMatrix = Handles.matrix;

            Handles.color = GetDebugColor();
            Handles.matrix = area.OBB.GetUnitBoxTransform();
            Handles.DrawWireCube(Vector3.zero, Vector3.one);

            Handles.matrix = oldMatrix;
            Handles.color = oldColor;
        }

        private static Color GetDebugColor()
        {
            Color color = GetBrushColor();
            return new Color(color.r, color.g, color.b, Mathf.Max(color.a, 0.85f));
        }*/
    }
}
#endif
