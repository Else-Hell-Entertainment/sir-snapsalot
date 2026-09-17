using EHE.Global.Config;
using EHE.Global.Logging;
using EHE.Global.Managers;

namespace EHE.Global.SceneManagement
{
    public class SceneTransition
    {
        public string TargetScenePath { get; }

        public SceneCollection SceneCollection { get; }

        public string SceneKey { get; }

        public SceneManager.SceneTransitionType SceneTransitionType { get; private set; } =
            SceneManager.SceneTransitionType.Default;

        public SceneTransitionPayload Payload { get; private set; } = null;

        public SceneTransition(SceneCollections.Collections sceneCollection, string sceneKey)
        {
            SceneCollection = SceneCollections.GetSceneCollection(sceneCollection);
            if (SceneCollection == null)
            {
                this.LogError($"SceneCollection {sceneCollection} not found");
                return;
            }

            TargetScenePath = SceneCollection.GetScenePath(sceneKey);
            SceneKey = sceneKey;
            if (TargetScenePath == null)
            {
                this.LogError($"Scene with key {sceneKey} not found in collection {SceneCollection.CollectionName}.");
            }
        }

        public SceneTransition(string sceneFilePath)
        {
            TargetScenePath = sceneFilePath;
        }

        public void SetPayload(SceneTransitionPayload payload)
        {
            Payload = payload;
        }

        public void SetTransitionType(SceneManager.SceneTransitionType transitionType)
        {
            SceneTransitionType = transitionType;
        }
    }
}
