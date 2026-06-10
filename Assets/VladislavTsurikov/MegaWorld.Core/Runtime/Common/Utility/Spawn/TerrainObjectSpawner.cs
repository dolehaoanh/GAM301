#if RENDERER_STACK
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using Instance = VladislavTsurikov.UnityUtility.Runtime.Instance;
using Random = UnityEngine.Random;
using TextureUtility = VladislavTsurikov.UnityUtility.Runtime.TextureUtility;
using PrototypeTerrainObject =
    VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject.
    PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
#if UNITY_EDITOR
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;
#endif
namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn
{
    public static class TerrainObjectSpawner
    {
        public static async UniTask ForEachSelectedPrototypeSampleAsync(Group group, BoxArea boxArea,
            Action<PrototypeTerrainObject, RayHit> spawnSample, Func<bool> shouldSkipSample = null,
            bool requireAreaContains = false, CancellationToken token = default,
            WaitingNextFrame waitingNextFrame = null)
        {
            ScatterComponentSettings scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(waitingNextFrame);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            await scatterComponentSettings.ScatterStack.Samples(boxArea, sample =>
            {
                token.ThrowIfCancellationRequested();

                if (shouldSkipSample != null && shouldSkipSample())
                {
                    return;
                }

                RayHit rayHit = RaycastUtility.Raycast(
                    RayUtility.GetRayDown(new Vector3(sample.x, sample.y, sample.z)),
                    layerSettings.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit == null)
                {
                    return;
                }

                if (requireAreaContains && !boxArea.Contains(rayHit.Point))
                {
                    return;
                }

                PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(
                    group.GetAllSelectedPrototypes());

                if (proto == null || !proto.Active)
                {
                    return;
                }

                spawnSample(proto, rayHit);
            }, token);
        }

        public static async UniTask SpawnGroupAsync(CancellationToken token, Group group, BoxArea boxArea,
            bool displayProgressBar, Func<bool> shouldSkipSample = null, bool useSelectedPrototypes = false,
            bool supportUndo = false, bool requireAreaContains = false)
        {
            if (useSelectedPrototypes)
            {
                FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

                if (filterSettings.FilterType == FilterType.MaskFilter)
                {
                    FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
                }
            }

            ScatterComponentSettings scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));

            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(displayProgressBar
                ? new DefaultWaitingNextFrame(0.2f)
                : null);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            await scatterComponentSettings.ScatterStack.Samples(boxArea, sample =>
            {
                token.ThrowIfCancellationRequested();

                if (shouldSkipSample != null && shouldSkipSample())
                {
                    return;
                }

                RayHit rayHit = RaycastUtility.Raycast(
                    RayUtility.GetRayDown(new Vector3(sample.x, sample.y, sample.z)),
                    layerSettings.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit == null)
                {
                    return;
                }

                if (requireAreaContains && !boxArea.Contains(rayHit.Point))
                {
                    return;
                }

                IReadOnlyList<Prototype> prototypeList = useSelectedPrototypes
                    ? group.GetAllSelectedPrototypes()
                    : group.PrototypeList;
                PrototypeTerrainObject proto =
                    (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(prototypeList);

                if (proto == null || !proto.Active)
                {
                    return;
                }

                float fitness = useSelectedPrototypes
                    ? SpawnFitness.Get(group, boxArea.Bounds, rayHit) *
                      TextureUtility.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask)
                    : SpawnFitness.Get(group, rayHit);

                if (fitness == 0 || Random.Range(0f, 1f) < 1 - fitness)
                {
                    return;
                }

                SpawnPrototype(proto, rayHit, fitness, supportUndo, requireAreaContains ? boxArea : null);
            }, token);
        }

        public static void SpawnPrototype(PrototypeTerrainObject proto, RayHit rayHit, float fitness,
            bool supportUndo = false, BoxArea area = null)
        {
            OverlapCheckSettings overlapCheckSettings =
                (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            Instance instance = new(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TransformComponentSettings transformComponentSettings =
                (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            if (area != null && !area.Contains(instance.Position))
            {
                return;
            }

            if (OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instance))
            {
                TerrainObjectInstance terrainObjectInstance =
                    TerrainObjectRendererAPI.AddInstance(proto.RendererPrototype, instance.Position, instance.Scale,
                        instance.Rotation);

#if UNITY_EDITOR
                if (supportUndo)
                {
                    Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObject(terrainObjectInstance));
                }
#endif
            }
        }
    }
}
#endif
