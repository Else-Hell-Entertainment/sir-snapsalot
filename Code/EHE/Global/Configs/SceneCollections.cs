using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.SceneManagement
{
    /// <summary>
    /// Contains constants and methods for managing scene collections in the game.
    /// Scene collections are used to group related scenes together, such as menu scenes or level scenes. This class
    /// provides methods to retrieve scene collections. The collections are designed to be used in the editor for scene
    /// transitions and contain keys for each scene in the collection.
    /// </summary>
    public static class SceneCollections
    {
        public enum Collections
        {
            None,
            MenuScenes,
            LevelScenes,
        }

        public static SceneCollection GetSceneCollection(Collections collection)
        {
            string path = "";
            switch (collection)
            {
                case Collections.MenuScenes:
                    path = MenuScenes.CollectionResourcePath;
                    break;
                case Collections.LevelScenes:
                    path = LevelScenes.CollectionResourcePath;
                    break;
            }

            SceneCollection coll = GD.Load<SceneCollection>(path);
            if (coll == null)
            {
                SceneManager.Instance.LogWarning(
                    $"No scene collection found for {path} using collection key {collection}."
                );

                return null;
            }

            return coll;
        }

        #region Scene Collection definitions

        /*
         * public static class FooScenes
         * {
         *     Don't change the variable name, use CollectionResourcePath for all scene classes for clarity.
         *     This path must always be defined! Otherwise, the collection will not be found.
         *
         *     public const string CollectionResourcePath =
         *         "res://Data/Resources/SceneCollections/FooSceneCollection.tres";
         *
         *     <summary>
         *     Keys must be manually matched to the keys in the scene collection resource. If you want to access a scene
         *     in code, you need to manually add the correct key here and make sure it matches the key in the resource!
         *     If it's not added here, the transitions will still work if defined in the editor.
         *     </summary>
         *
         *     public static class Keys
         *      {
         *        public const string Bar = "Bar";
         *      }
         * }
         */

        public static class MenuScenes
        {
            public const string CollectionResourcePath =
                "res://Data/Resources/SceneCollections/MenuSceneCollection.tres";

            public static class Keys
            {
                public const string MainMenuRoot = "MainMenuRoot";
                public const string MainMenu = "MainMenu";
                public const string Pause = "Pause";
                public const string Settings = "Settings";
                public const string AudioSettings = "AudioSettings";
                public const string VideoSettings = "VideoSettings";
                public const string GameplaySettings = "GamePlaySettings";
                public const string ControlSettings = "ControlSettings";
                public const string PauseMenuRoot = "PauseMenuRoot";
            }
        }

        public static class LevelScenes
        {
            public const string CollectionResourcePath =
                "res://Data/Resources/SceneCollections/LevelSceneCollection.tres";

            public static class Keys
            {
                public const string World = "World";
            }
        }

        #endregion
    }
}
