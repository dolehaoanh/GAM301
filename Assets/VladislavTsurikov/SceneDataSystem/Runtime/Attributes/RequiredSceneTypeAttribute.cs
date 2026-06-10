namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    public class RequiredSceneTypeAttribute : AllowCreateComponentAttribute
    {
        private readonly SceneType _sceneType;

        public RequiredSceneTypeAttribute(SceneType sceneType) => _sceneType = sceneType;

        public override bool Allow(SceneDataManager sceneDataManager)
        {
            return _sceneType == sceneDataManager.SceneType;
        }
    }
}
