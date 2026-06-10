using System.Collections.Generic;
using UnityEditor;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public static class MeshStack
    {
        private static Dictionary<UnityEngine.Mesh, Mesh> _unityMeshToMesh = new();

#if UNITY_EDITOR
        static MeshStack()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
            EditorApplication.projectChanged += OnProjectChanged;
        }
#endif

        public static Mesh GetEditorMesh(UnityEngine.Mesh unityMesh)
        {
            if (_unityMeshToMesh.TryGetValue(unityMesh, out Mesh mesh))
            {
                return mesh;
            }

            Mesh editorMesh = new(unityMesh);
            _unityMeshToMesh.Add(unityMesh, editorMesh);
            return editorMesh;
        }

        private static void OnProjectChanged()
        {
            Dictionary<UnityEngine.Mesh, Mesh> newDictionary = new();
            foreach (KeyValuePair<UnityEngine.Mesh, Mesh> pair in _unityMeshToMesh)
            {
                if (pair.Key != null)
                {
                    newDictionary.Add(pair.Key, pair.Value);
                }
            }

            _unityMeshToMesh.Clear();
            _unityMeshToMesh = newDictionary;
        }
    }
}
