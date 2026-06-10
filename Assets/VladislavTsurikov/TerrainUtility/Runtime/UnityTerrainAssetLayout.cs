using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public static class UnityTerrainAssetLayout
    {
        public const string DefaultRoot = "Assets/WorldCreatorTerrains";

        public static string GetTerrainRoot(string terrainFolderName)
        {
            string safeName = SanitizeFileName(terrainFolderName, "Terrain");
            return $"{DefaultRoot}/{safeName}";
        }

        public static string GetTerrainDataFolder(string terrainFolderName)
        {
            return GetTerrainRoot(terrainFolderName);
        }

        public static string GetTerrainDataAssetPath(string terrainFolderName, string tileName)
        {
            return $"{GetTerrainDataFolder(terrainFolderName)}/{tileName}.asset";
        }

        public static string GetMaskFolder(string terrainFolderName)
        {
            return $"{GetTerrainRoot(terrainFolderName)}/Masks";
        }

        public static string GetMaskAssetPath(string terrainFolderName, string tileName, string maskId)
        {
            string safeTileName = SanitizeFileName(tileName, "Tile");
            string safeMaskId = SanitizeFileName(maskId, "Mask");
            return $"{GetMaskFolder(terrainFolderName)}/{safeTileName}__{safeMaskId}.asset";
        }

        public static void EnsureAssetFolder(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            string[] parts = assetFolderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        public static string SanitizeFileName(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            char[] safeCharacters = value.ToCharArray();

            for (int i = 0; i < safeCharacters.Length; i++)
            {
                if (Array.IndexOf(invalidCharacters, safeCharacters[i]) >= 0)
                {
                    safeCharacters[i] = '_';
                }
            }

            string safeValue = new string(safeCharacters).Trim();
            return string.IsNullOrWhiteSpace(safeValue) ? fallback : safeValue;
        }

        public static string BuildFolderName(string displayName, string normalizedTerrainFilePath)
        {
            string safeName = SanitizeFileName(displayName, "Terrain");
            string hash = ComputeShortHash(normalizedTerrainFilePath);
            return string.IsNullOrWhiteSpace(hash) ? safeName : $"{safeName}_{hash}";
        }

        private static string ComputeShortHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            string hex = BitConverter.ToString(hash).Replace("-", string.Empty);
            return hex.Substring(0, 8);
        }
    }
}
