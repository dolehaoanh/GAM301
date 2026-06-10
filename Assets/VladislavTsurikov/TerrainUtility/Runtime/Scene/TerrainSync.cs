using UnityEngine;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public sealed class TerrainSync : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private string _terrainFilePath = string.Empty;

        [SerializeField]
        [HideInInspector]
        private string _displayName = string.Empty;

        [SerializeField]
        [HideInInspector]
        private string _generatedAssetFolderPath = string.Empty;

        [SerializeField]
        [HideInInspector]
        private string _lastBuildFolderPath = string.Empty;

        public string TerrainFilePath => _terrainFilePath;
        public string DisplayName => _displayName;
        public string GeneratedAssetFolderPath => _generatedAssetFolderPath;
        public string LastBuildFolderPath => _lastBuildFolderPath;

        public void Configure(string terrainFilePath, string displayName, string generatedAssetFolderPath,
            string lastBuildFolderPath)
        {
            _terrainFilePath = terrainFilePath ?? string.Empty;
            _displayName = displayName ?? string.Empty;
            _generatedAssetFolderPath = generatedAssetFolderPath ?? string.Empty;
            _lastBuildFolderPath = lastBuildFolderPath ?? string.Empty;
        }
    }
}
