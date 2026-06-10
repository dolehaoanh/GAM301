#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Templates
{
    [Template("Trees/Big Trees",
        new[] { "Object Brush", "Object Stamper" },
        new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    public class BigTrees : Template
    {
        protected override void Apply(Group group)
        {
            ScatterComponentSettings scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            #region Scatter Settings

            scatterComponentSettings.ScatterStack.Clear();

            RandomGrid randomGrid =
                (RandomGrid)scatterComponentSettings.ScatterStack.CreateIfMissingType(typeof(RandomGrid));

            randomGrid.RandomisationType = RandomisationType.Square;
            randomGrid.Vastness = 1;
            randomGrid.GridStep = new Vector2(3, 3);
            randomGrid.FailureRate = 80;

            #endregion

            #region Mask Filters

            filterSettings.FilterType = FilterType.MaskFilter;
            filterSettings.MaskFilterComponentSettings.MaskFilterStack.Clear();

            HeightFilter heightFilter =
                (HeightFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateNode(
                    typeof(HeightFilter));
            heightFilter.MinHeight = 0;
            heightFilter.MaxHeight = 620;
            heightFilter.AddHeightFalloff = 100;

            filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateNode(typeof(SlopeFilter));

            MaskOperationsFilter maskOperationsFilter =
                (MaskOperationsFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateNode(
                    typeof(MaskOperationsFilter));
            maskOperationsFilter.MaskOperations = MaskOperations.Remap;
            maskOperationsFilter.RemapRange = new Vector2(0.8f, 1f);

            #endregion
        }

        protected override void Apply(Prototype proto)
        {
            TransformComponentSettings transformComponentSettings =
                (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            OverlapCheckSettings overlapCheckSettings =
                (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            #region Transform Components

            transformComponentSettings.TransformComponentStack.Clear();

            transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(TreeRotation));
            transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(Align));
            transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(PositionOffset));
            transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(SlopePosition));
            transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(Scale));
            ScaleFitness scaleFitness =
                (ScaleFitness)transformComponentSettings.TransformComponentStack.CreateIfMissingType(
                    typeof(ScaleFitness));
            scaleFitness.OffsetScale = -1.2f;

            transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(ScaleClamp));

            #endregion

            #region OverlapCheckSettings

            overlapCheckSettings.OverlapShapeEnum = OverlapShapeEnum.Sphere;
            overlapCheckSettings.SphereCheck.VegetationMode = true;
            overlapCheckSettings.SphereCheck.Priority = 0;
            overlapCheckSettings.SphereCheck.ViabilitySize = 4f;
            overlapCheckSettings.SphereCheck.TrunkSize = 0.8f;

            #endregion
        }
    }
}
#endif
