using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Name("Tiles")]
    [MegaWorldDocUrl("scatter-system/tiles")]
    public class Tiles : Scatter
    {
        public Vector2 Size = new(1, 1);

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector3> samples,
            Action<Vector3> onSpawn = null)
        {
            Vector2Int tileMapSize = new(
                Mathf.RoundToInt(boxArea.BoxSize),
                Mathf.RoundToInt(boxArea.BoxSize)
            );

            for (int x = 0; x < tileMapSize.x; x++)
            for (int y = 0; y < tileMapSize.y; y++)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                Vector3 cellCenter = new(
                    x * Size.x + Size.x / 2,
                    boxArea.Center.y,
                    y * Size.y + Size.y / 2
                );

                if (boxArea.Contains(cellCenter))
                {
                    onSpawn?.Invoke(cellCenter);
                    samples.Add(cellCenter);
                }
            }
        }
    }
}
