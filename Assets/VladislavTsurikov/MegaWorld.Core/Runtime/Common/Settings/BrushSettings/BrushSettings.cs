using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.AdvancedBrushSettings
{
    public enum MaskType
    {
        Custom,
        Procedural
    }

    public enum SpacingEqualsType
    {
        BrushSize,
        HalfBrushSize,
        Custom
    }

    [Name("Brush Settings")]
    public class AdvancedBrushSettings : BrushSettings
    {
        [OdinSerialize]
        private float _customCustomSpacing = 30;

        public BrushJitterSettings BrushJitterSettings = new();
        public CustomMasks CustomMasks = new();
        public MaskType MaskType = MaskType.Procedural;

        public ProceduralMask ProceduralMask = new();
        public SpacingEqualsType SpacingEqualsType = SpacingEqualsType.HalfBrushSize;

        public float CustomSpacing
        {
            get => _customCustomSpacing;
            set => _customCustomSpacing = Mathf.Max(0.01f, value);
        }

        public override float Spacing
        {
            get
            {
                switch (SpacingEqualsType)
                {
                    case SpacingEqualsType.BrushSize:
                    {
                        return BrushSize;
                    }
                    case SpacingEqualsType.HalfBrushSize:
                    {
                        return BrushSize / 2;
                    }
                    default:
                    {
                        return Mathf.Max(0.01f, CustomSpacing);
                    }
                }
            }
        }

        public override Texture2D GetCurrentRaw()
        {
            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.GetMask(Shape);

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }
    }
}
