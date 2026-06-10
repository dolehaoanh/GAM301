using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Name("Failure Rate")]
    [MegaWorldDocUrl("scatter-system/failure-rate")]
    public class FailureRate : Scatter
    {
        [Range(0f, 100f)]
        [Tooltip("Percentage of samples to remove (0-100%)")]
        public float Value = 70;

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector3> samples,
            Action<Vector3> onSpawn = null)
        {
            for (int i = samples.Count - 1; i >= 0; i--)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                if (Random.Range(0f, 100f) < Value)
                {
                    samples.RemoveAt(i);
                }
                else
                {
                    onSpawn?.Invoke(samples[i]);
                }
            }
        }
    }
}
