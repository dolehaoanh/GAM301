using System.IO;
using UnityEditor;

namespace VladislavTsurikov.Utility.Editor
{
    public class CsFileUtility
    {
        public static string FindPath(string name)
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
