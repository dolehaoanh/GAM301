#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;

namespace VladislavTsurikov.UIElementsUtility.Editor.Core
{
    public abstract class DataGroupScriptableObjectCreator<T, N> where T : DataGroup<T, N>
    {
        public void Run()
        {
            string defaultColorPaletteName = typeof(T).GetAttribute<CreateAssetMenuAttribute>().fileName;
            string assetUserName = GetGroupName();
            string assetName = defaultColorPaletteName + "_" + assetUserName;
            string defaultColorPalettePath = GetDefaultDataGroupPath(assetName);

            T data = AssetDatabase.LoadAssetAtPath<T>(defaultColorPalettePath);
            bool defaultPaletteNotFound = data == null;
            if (defaultPaletteNotFound)
            {
                data = ScriptableObject.CreateInstance<T>();
            }

            data._groupName = assetUserName;
            data._assetDefaultName = defaultColorPaletteName;
            data.name = assetName;

            if (defaultPaletteNotFound)
            {
                AssetDatabase.CreateAsset(data, defaultColorPalettePath);
            }

            AddItems(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }

        private string GetDefaultDataGroupPath(string assetName)
        {
            string folderPath = GetScriptableObjectGenerationPath();

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string defaultColorPalettePath = Path.Combine(folderPath, assetName + ".asset");
            return defaultColorPalettePath;
        }

        protected virtual string GetGroupName()
        {
            return "Default";
        }

        protected abstract void AddItems(T data);
        protected abstract string GetScriptableObjectGenerationPath();
    }
}
#endif
