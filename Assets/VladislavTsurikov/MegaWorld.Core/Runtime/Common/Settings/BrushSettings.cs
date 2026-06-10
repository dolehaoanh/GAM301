using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    public enum Shape
    {
        Circle,
        Square
    }

    [Name("Brush Settings")]
    [MegaWorldDocUrl("shared-settings/brush-settings")]
    public class BrushSettings : Node
    {
        [OdinSerialize]
        private float _brushSize = 100;

        [OdinSerialize]
        private float _brushSpacing = 30;

        [OdinSerialize]
        public Shape Shape = Shape.Circle;

        public virtual float Spacing
        {
            get => _brushSpacing;
            set => _brushSpacing = Mathf.Max(0.01f, value);
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = Mathf.Max(0.01f, value);
        }

        public float BrushRadius => _brushSize / 2;

        public virtual Texture2D GetCurrentRaw()
        {
            return Texture2D.whiteTexture;
        }

        public void ScrollBrushRadiusEvent()
        {
            if (Event.current.shift && Event.current.type == EventType.ScrollWheel)
            {
                float scrollDelta = Mathf.Abs(Event.current.delta.y) > Mathf.Epsilon
                    ? Event.current.delta.y
                    : Event.current.delta.x;

                if (Mathf.Abs(scrollDelta) > Mathf.Epsilon)
                {
                    BrushSize += scrollDelta;
                    Event.current.Use();
                }
            }
        }

        public BoxArea GetAreaVariables(RayHit hit)
        {
            if (hit == null)
            {
                return null;
            }

            Shape areaShape = Shape;
            BoxArea area = new(hit, BrushSize) { Mask = GetCurrentRaw(), Shape = areaShape };

            return area;
        }
    }
}
