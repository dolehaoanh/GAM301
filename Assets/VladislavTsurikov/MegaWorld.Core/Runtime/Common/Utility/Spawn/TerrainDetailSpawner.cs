using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn
{
    public static class TerrainDetailSpawner
    {
        public static async UniTask SpawnAreaAsync(CancellationToken token, Group group,
            IReadOnlyList<Prototype> protoTerrainDetailList, BoxArea boxArea, Terrain terrain,
            bool waitNextFrame = false)
        {
            if (terrain == null)
            {
                return;
            }

            if (TerrainResourcesController.IsSyncError(group, terrain))
            {
                return;
            }

            if (terrain.terrainData.detailPrototypes.Length == 0)
            {
                Debug.LogWarning("Add Terrain Details");
                return;
            }

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                token.ThrowIfCancellationRequested();

                if (!proto.Active)
                {
                    continue;
                }

                SpawnPrototype(proto, boxArea, terrain);

                if (waitNextFrame)
                {
                    await UniTask.Yield();
                }
            }

            if (waitNextFrame)
            {
                await UniTask.Yield();
            }
        }

        private static void SpawnPrototype(PrototypeTerrainDetail proto, BoxArea boxArea, Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            Vector2Int spawnSize = new(
                UnityTerrainUtility.WorldToDetail(boxArea.Size.x, terrainData.size.x, terrainData),
                UnityTerrainUtility.WorldToDetail(boxArea.Size.z, terrainData.size.z, terrainData));

            Vector2Int halfBrushSize = spawnSize / 2;

            Vector2Int center = new(
                UnityTerrainUtility.WorldToDetail(boxArea.Center.x - terrain.transform.position.x, terrain.terrainData),
                UnityTerrainUtility.WorldToDetail(boxArea.Center.z - terrain.transform.position.z,
                    terrain.terrainData.size.z, terrain.terrainData));

            Vector2Int position = center - halfBrushSize;
            Vector2Int startPosition = Vector2Int.Max(position, Vector2Int.zero);
            Vector2Int offset = startPosition - position;

            float detailMapResolution = terrain.terrainData.detailResolution;

            int[,] localData = terrain.terrainData.GetDetailLayer(
                startPosition.x, startPosition.y,
                Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailMapResolution) - startPosition.x),
                Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailMapResolution) - startPosition.y),
                proto.TerrainProtoId);

            float widthY = localData.GetLength(1);
            float heightX = localData.GetLength(0);

            MaskFilterComponentSettings maskFilterComponentSettings =
                (MaskFilterComponentSettings)proto.GetElement(typeof(MaskFilterComponentSettings));
            SpawnDetailSettings spawnDetailSettings =
                (SpawnDetailSettings)proto.GetElement(typeof(SpawnDetailSettings));

            for (int y = 0; y < widthY; y++)
            for (int x = 0; x < heightX; x++)
            {
                Vector2Int current = new(y, x);

                Vector2 normal = current + startPosition;
                normal /= detailMapResolution;

                Vector2 worldPosition = UnityTerrainUtility.GetTerrainWorldPositionFromRange(normal, terrain);

                float fitness = SpawnFitness.Get(maskFilterComponentSettings, terrain,
                    new Vector3(worldPosition.x, 0, worldPosition.y));
                float maskFitness = boxArea.GetAlpha(current + offset, spawnSize);
                float targetFitness = fitness * maskFitness;

                int targetStrength;

                if (spawnDetailSettings.UseRandomOpacity)
                {
                    float randomFitness = targetFitness;
                    randomFitness *= Random.value;

                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, randomFitness));
                }
                else
                {
                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, targetFitness));
                }

                if (Random.Range(0f, 1f) < 1 - targetFitness ||
                    Random.Range(0f, 1f) < spawnDetailSettings.FailureRate / 100)
                {
                    targetStrength = 0;
                }

                localData[x, y] = targetStrength;
            }

            terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
        }
    }
}
