using System;
using System.IO;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.SceneUtility.Editor.Utility
{
    public static class BuildSceneUtility
    {
        public static bool IsSceneInBuildSettings(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                string name = Path.GetFileNameWithoutExtension(path);

                if (string.Equals(name, sceneName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
