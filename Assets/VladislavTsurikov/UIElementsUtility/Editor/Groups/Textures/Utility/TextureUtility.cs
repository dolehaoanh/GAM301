#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Textures
{
    public static class TextureUtility
    {
        public static void ReimportTexturesToGUITextures(IEnumerable<string> filePaths)
        {
            if (filePaths == null)
            {
                return;
            }

            AssetDatabase.StartAssetEditing();

            foreach (string filePath in filePaths)
            {
                ReimportTextureToGUITexture(filePath);
            }

            AssetDatabase.StopAssetEditing();
        }

        public static void ReimportTextureToGUITexture(string filePath)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }

            Debug.Assert(textureImporter != null, nameof(textureImporter) + " != null");
            bool requiresImport = false;
            if (textureImporter.textureType != TextureImporterType.GUI)
            {
                textureImporter.textureType = TextureImporterType.GUI;
                requiresImport = true;
            }

            if (!textureImporter.mipmapEnabled)
            {
                textureImporter.mipmapEnabled = true;
                requiresImport = true;
            }

            if (textureImporter.filterMode != FilterMode.Trilinear)
            {
                textureImporter.filterMode = FilterMode.Trilinear;
                requiresImport = true;
            }

            if (textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                requiresImport = true;
            }

            if (!requiresImport)
            {
                return;
            }

            Debug.Log($"Importing: {filePath}");

            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        }
    }
}
#endif
