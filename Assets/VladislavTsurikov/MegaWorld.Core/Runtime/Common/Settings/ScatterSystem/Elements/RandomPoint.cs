using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;
using AreaShape = VladislavTsurikov.MegaWorld.Runtime.Common.Settings.Shape;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Name("Random Point")]
    [MegaWorldDocUrl("scatter-system/random-point")]
    public class RandomPoint : Scatter
    {
        [HideInInspector]
        public int MaxChecks = 15;

        [MinMaxSlider(1f, 15f, nameof(MaxChecks), MaxValueMemberName = nameof(MaxChecksLimit),
            LabelOverride = "Checks")]
        public int MinChecks = 15;

        public int MaxChecksLimit => PreferenceElementSingleton<ScatterPreferenceSettings>.Instance.MaxChecks;

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector3> samples,
            Action<Vector3> onSpawn = null)
        {
            int numberOfChecks = Random.Range(MinChecks, MaxChecks);

            for (int checks = 0; checks < numberOfChecks; checks++)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                Vector3 point = GetRandomPoint(boxArea);

                if (!boxArea.Contains(point))
                {
                    continue;
                }

                onSpawn?.Invoke(point);
                samples.Add(point);
            }
        }

        private Vector3 GetRandomPoint(BoxArea boxArea)
        {
            if (boxArea.Shape == AreaShape.Circle)
            {
                Vector2 spawnOffset = Random.insideUnitCircle * boxArea.Radius;
                return boxArea.Center + boxArea.Tangent * spawnOffset.x + boxArea.Bitangent * spawnOffset.y;
            }

            Vector2 squareOffset = new(
                Random.Range(-boxArea.Radius, boxArea.Radius),
                Random.Range(-boxArea.Radius, boxArea.Radius));
            return boxArea.Center + boxArea.Tangent * squareOffset.x + boxArea.Bitangent * squareOffset.y;
        }
    }
}
