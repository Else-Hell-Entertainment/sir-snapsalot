// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using EHE.SirSnapsalot.SceneManagement;

namespace EHE.SirSnapsalot.Config
{
    public class GameStartConfig
    {
        public readonly string DefaultPlayerId = "DEFAULT PLAYER";

        public SceneCollections.Collections StartingSceneCollection { get; set; } =
            SceneCollections.Collections.LevelScenes;

        public string DefaultSceneKey { get; set; }

        public GameStartConfig() { }
    }
}
