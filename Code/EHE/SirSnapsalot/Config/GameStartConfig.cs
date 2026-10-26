using EHE.SirSnapsalot.SceneManagement;

namespace EHE.SirSnapsalot.Config
{
    public class GameStartConfig
    {
        public string DefaultPlayerId = "DEFAULT PLAYER";

        public SceneCollections.Collections StartingSceneCollection { get; set; } =
            SceneCollections.Collections.LevelScenes;

        public string DefaultSceneKey { get; set; }

        public GameStartConfig() { }
    }
}
