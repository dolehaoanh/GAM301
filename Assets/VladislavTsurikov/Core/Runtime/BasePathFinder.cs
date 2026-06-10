using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.Core.Runtime
{
    public class BasePathFinder<T> where T : class
    {
        private static string s_foundPath = string.Empty;

        public static string Path
        {
            get
            {
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(s_foundPath))
                    return s_foundPath;

                string typeName = typeof(T).Name;
                string[] guids = AssetDatabase.FindAssets($"t:MonoScript {typeName}");
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                    if (script != null && script.GetClass() == typeof(T))
                    {
                        string dir = System.IO.Path.GetDirectoryName(assetPath);
                        s_foundPath = dir != null ? dir.Replace('\\', '/') : assetPath.Replace('\\', '/');
                        return s_foundPath;
                    }
                }

                Debug.LogError($"[BasePathFinder] Could not find path for type '{typeName}'. Make sure the script name matches the class name.");
                return string.Empty;
#else
                return string.Empty;
#endif
            }
        }
    }
}
