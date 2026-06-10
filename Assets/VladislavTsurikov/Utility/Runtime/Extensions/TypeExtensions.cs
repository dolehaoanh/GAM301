#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class TypeExtensions
    {
        public static string GetSourceFilePath(this Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return FindCsFilePath(type.Name);
        }

        public static string FindCsFilePath(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"{name} t:Script");
            if (guids == null || guids.Length == 0)
            {
                return string.Empty;
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName == name)
                {
                    return path;
                }
            }

            return string.Empty;
        }
    }
}
#endif
