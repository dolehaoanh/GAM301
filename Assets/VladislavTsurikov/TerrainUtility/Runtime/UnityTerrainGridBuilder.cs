using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public sealed class UnityTerrainGridBuilder
    {
        public UnityTerrainBuildResult Build(UnityTerrainGridSettings settings,
            IReadOnlyList<UnityTerrainTileData> tiles)
        {
            string safeTerrainName = UnityTerrainAssetLayout.SanitizeFileName(settings.TerrainAssetName, "Terrain");
            string normalizedTerrainFilePath = NormalizePath(settings.TerrainFilePath);
            TerrainSync sync = PrepareSync(settings, safeTerrainName, normalizedTerrainFilePath,
                out string terrainFolderName,
                out GameObject container);
            string assetRoot = UnityTerrainAssetLayout.GetTerrainRoot(terrainFolderName);

            UnityTerrainAssetLayout.EnsureAssetFolder(assetRoot);
            UnityTerrainAssetLayout.EnsureAssetFolder(UnityTerrainAssetLayout.GetMaskFolder(terrainFolderName));
            Dictionary<Vector2Int, Terrain> terrains = new();
            Dictionary<Vector2Int, UnityTerrainTileData> tilesByCoordinate = CreateTileLookup(tiles);
            UnityTerrainBuildResult result = new() { Container = container, Sync = sync, AssetFolderPath = assetRoot };

            float tileWorldSize = settings.WorldWidth / Mathf.Max(1, settings.TilesX);

            for (int i = 0; i < tiles.Count; i++)
            {
                UnityTerrainTileData tile = tiles[i];
                if (tile?.Heights == null)
                {
                    continue;
                }

                string tileName = $"{safeTerrainName}_{tile.TileX}_{tile.TileY}";
                string terrainDataPath = UnityTerrainAssetLayout.GetTerrainDataAssetPath(terrainFolderName, tileName);
                TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataPath);

                if (terrainData == null)
                {
                    terrainData = new TerrainData();
                    terrainData.name = tileName;
                    AssetDatabase.CreateAsset(terrainData, terrainDataPath);
                }

                Vector2Int tileKey = new(tile.TileX, tile.TileY);
                float[,] rightNeighborHeights = TryGetNeighborHeights(tilesByCoordinate, tileKey + Vector2Int.right);
                float[,] topNeighborHeights = TryGetNeighborHeights(tilesByCoordinate, tileKey + Vector2Int.up);
                float[,] topRightNeighborHeights = TryGetNeighborHeights(tilesByCoordinate, tileKey + Vector2Int.one);

                UnityTerrainHeightApplicator.ApplyHeights(terrainData, tile.Heights, rightNeighborHeights,
                    topNeighborHeights,
                    topRightNeighborHeights);
                terrainData.alphamapResolution = terrainData.heightmapResolution - 1;
                terrainData.size = new Vector3(tileWorldSize, settings.WorldHeight, tileWorldSize);

                Terrain terrain = CreateOrUpdateTerrain(container.transform, tileName, terrainData);
                terrain.transform.position = new Vector3(tile.TileX * tileWorldSize, 0f, tile.TileY * tileWorldSize);
                result.ImportedMaskCount +=
                    ConfigureAreaMask(terrain.gameObject, terrainFolderName, tileName, tile.Masks);

                terrains[new Vector2Int(tile.TileX, tile.TileY)] = terrain;
                result.ImportedTileCount++;
            }

            UnityTerrainNeighborUtility.ApplyNeighbors(terrains, settings.TilesX, settings.TilesY);
            sync.Configure(normalizedTerrainFilePath, safeTerrainName, assetRoot,
                NormalizePath(settings.BuildFolderPath));
            EditorUtility.SetDirty(sync);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return result;
        }

        private static Dictionary<Vector2Int, UnityTerrainTileData> CreateTileLookup(
            IReadOnlyList<UnityTerrainTileData> tiles)
        {
            Dictionary<Vector2Int, UnityTerrainTileData> tileLookup = new();

            for (int i = 0; i < tiles.Count; i++)
            {
                UnityTerrainTileData tile = tiles[i];
                if (tile?.Heights == null)
                {
                    continue;
                }

                tileLookup[new Vector2Int(tile.TileX, tile.TileY)] = tile;
            }

            return tileLookup;
        }

        private static float[,] TryGetNeighborHeights(Dictionary<Vector2Int, UnityTerrainTileData> tilesByCoordinate,
            Vector2Int key)
        {
            return tilesByCoordinate.TryGetValue(key, out UnityTerrainTileData tile) ? tile.Heights : null;
        }

        private static TerrainSync PrepareSync(UnityTerrainGridSettings settings, string safeTerrainName,
            string normalizedTerrainFilePath,
            out string terrainFolderName, out GameObject container)
        {
            TerrainSync[] syncs =
                Object.FindObjectsByType<TerrainSync>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            TerrainSync[] exactMatches = syncs.Where(sync =>
                    string.Equals(NormalizePath(sync.TerrainFilePath), normalizedTerrainFilePath,
                        StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (exactMatches.Length > 1)
            {
                throw new InvalidOperationException(
                    $"More than one terrain container is already synchronized with '{normalizedTerrainFilePath}'.");
            }

            TerrainSync exactMatch = exactMatches.Length == 1 ? exactMatches[0] : null;
            bool hasConflictingNameInScene = syncs.Any(sync =>
                sync != null &&
                sync != exactMatch &&
                string.Equals(sync.DisplayName, safeTerrainName, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
            {
                terrainFolderName = ResolveTerrainFolderName(safeTerrainName, normalizedTerrainFilePath,
                    hasConflictingNameInScene,
                    exactMatch.GeneratedAssetFolderPath);
                DeleteAssetFolder(exactMatch.GeneratedAssetFolderPath);
                ClearChildren(exactMatch.transform);
                container = exactMatch.gameObject;
                container.name = safeTerrainName;
                return exactMatch;
            }

            terrainFolderName = ResolveTerrainFolderName(safeTerrainName, normalizedTerrainFilePath,
                hasConflictingNameInScene, string.Empty);
            container = new GameObject(safeTerrainName);
            return container.AddComponent<TerrainSync>();
        }

        private static string ResolveTerrainFolderName(string safeTerrainName, string normalizedTerrainFilePath,
            bool hasConflictingNameInScene,
            string existingAssetFolderPath)
        {
            if (!string.IsNullOrWhiteSpace(existingAssetFolderPath))
            {
                return Path.GetFileName(existingAssetFolderPath.TrimEnd('/', '\\'));
            }

            string baseRoot = UnityTerrainAssetLayout.GetTerrainRoot(safeTerrainName);
            if (!hasConflictingNameInScene && !AssetDatabase.IsValidFolder(baseRoot))
            {
                return safeTerrainName;
            }

            return UnityTerrainAssetLayout.BuildFolderName(safeTerrainName, normalizedTerrainFilePath);
        }

        private static void DeleteAssetFolder(string assetFolderPath)
        {
            if (string.IsNullOrWhiteSpace(assetFolderPath) || !AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            AssetDatabase.DeleteAsset(assetFolderPath);
        }

        private static void ClearChildren(Transform container)
        {
            while (container.childCount > 0)
            {
                Object.DestroyImmediate(container.GetChild(0).gameObject);
            }
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return Path.GetFullPath(path).Replace('\\', '/');
        }

        private static Terrain CreateOrUpdateTerrain(Transform container, string tileName, TerrainData terrainData)
        {
            Transform tileTransform = container.Find(tileName);
            GameObject tileObject;

            if (tileTransform == null)
            {
                tileObject = Terrain.CreateTerrainGameObject(terrainData);
                tileObject.name = tileName;
                tileObject.transform.SetParent(container, false);
            }
            else
            {
                tileObject = tileTransform.gameObject;
            }

            Terrain terrain = tileObject.GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = tileObject.AddComponent<Terrain>();
            }

            TerrainCollider terrainCollider = tileObject.GetComponent<TerrainCollider>();
            if (terrainCollider == null)
            {
                terrainCollider = tileObject.AddComponent<TerrainCollider>();
            }

            terrain.terrainData = terrainData;
            terrainCollider.terrainData = terrainData;
            terrain.drawInstanced = true;
            terrain.heightmapPixelError = 1f;
            terrain.Flush();
            return terrain;
        }

        private static int ConfigureAreaMask(GameObject terrainObject, string terrainFolderName, string tileName,
            UnityTerrainTileMaskData[] masks)
        {
            AreaMask areaMask = terrainObject.GetComponent<AreaMask>();

            if (masks == null || masks.Length == 0)
            {
                if (areaMask != null)
                {
                    Object.DestroyImmediate(areaMask);
                }

                return 0;
            }

            areaMask ??= terrainObject.AddComponent<AreaMask>();

            List<AreaMaskEntry> entries = new(masks.Length);
            for (int i = 0; i < masks.Length; i++)
            {
                UnityTerrainTileMaskData maskData = masks[i];
                if (maskData?.Texture == null || string.IsNullOrWhiteSpace(maskData.Id))
                {
                    continue;
                }

                string maskAssetPath =
                    UnityTerrainAssetLayout.GetMaskAssetPath(terrainFolderName, tileName, maskData.Id);
                maskData.Texture.name = $"{tileName}_{maskData.Id}";
                AssetDatabase.CreateAsset(maskData.Texture, maskAssetPath);

                entries.Add(new AreaMaskEntry { Id = maskData.Id, Mask = maskData.Texture });
            }

            areaMask.SetMasks(entries);
            EditorUtility.SetDirty(areaMask);
            return entries.Count;
        }
    }
}
