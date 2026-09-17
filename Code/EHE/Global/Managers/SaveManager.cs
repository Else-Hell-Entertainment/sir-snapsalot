using System;
using System.IO;
using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.SaveSystem
{
    public enum SaveBackend
    {
        None = 0,
        Json,
        Binary,
        Mock,
    }

    public class SaveManager
    {
        private SaveBackend _backend = SaveBackend.None;
        private string _saveLocation = null;
        private ISaveSerializer _serializer = null;
        private string _quickSaveSlot = null;

        public SaveManager(SaveConfig config)
        {
            _saveLocation = config.SaveLocation;
            _quickSaveSlot = config.QuickSaveSlot;
            SetBackend(config.DefaultSaveBackend);
        }

        #region Public API

        /// <summary>
        /// Returns the slot names (filename without extension) of all saves on disk
        /// that match the current backend's file extension.
        /// </summary>
        public string[] ListSaves()
        {
            string directory = ProjectSettings.GlobalizePath(_saveLocation);
            if (!Directory.Exists(directory))
            {
                return Array.Empty<string>();
            }

            string[] files = Directory.GetFiles(directory, $"*.{_serializer.FileExtension}");
            return Array.ConvertAll(files, file => Path.GetFileNameWithoutExtension(file));
        }

        public void SetBackend(SaveBackend backend)
        {
            _backend = backend;
            _serializer = CreateSerializer(backend);
            GD.Print($"[SaveManager] Backend switched to {backend}.");
        }

        /// <summary>
        /// Snapshot the current game state and write it to disk.
        /// </summary>
        /// <param name="saveSlotName">Save slot identifier, used as the filename.</param>
        public void Save(string saveSlotName)
        {
            // Collect the save data.
            GameData data = CollectData();

            // Form the save file path.
            string path = BuildPath(saveSlotName);

            EnsureDirectoryExists(path);

            using (FileStream fileStream = File.Open(path, FileMode.Create, System.IO.FileAccess.Write))
            {
                _serializer.Serialize(fileStream, data);
                GD.Print($"[SaveManager] Game saved to {path} using {_backend}.");
            }
        }

        /// <summary>
        /// Load a previously saved slot from disk and apply its state to the game objects.
        /// </summary>
        /// <param name="saveSlotName">Save slot identifier matching the one used in <see cref="Save"/>.</param>
        /// <returns><c>true</c> if the file was found and applied; <c>false</c> if no save exists.</returns>
        public bool Load(string saveSlotName)
        {
            string path = BuildPath(saveSlotName);

            if (!File.Exists(path))
            {
                GD.Print($"[SaveManager] No save file found at {path}.");
                return false;
            }

            using (FileStream fileStream = File.Open(path, FileMode.Open, System.IO.FileAccess.Read))
            {
                GameData gameData = _serializer.Deserialize(fileStream);
                ApplyState(gameData);
                GD.Print($"[SaveManager] Game loaded from {path} using {_backend}.");
            }

            return true;
        }

        public void QuickSave()
        {
            Save(_quickSaveSlot);
        }

        public void QuickLoad()
        {
            Load(_quickSaveSlot);
        }

        public void LoadGameData(GameData data)
        {
            ApplyState(data);
            this.LogInfo($"Game data loaded from provided data object.");
        }

        #endregion Public API

        #region Internal functionality

        private GameData CollectData()
        {
            GameData data = new GameData();
            data.PlayerData = PlayerManager.Instance.SaveState();

            return data;
        }

        private void ApplyState(GameData data)
        {
            PlayerManager.Instance.LoadState(data.PlayerData);
            SceneManager.Instance.LoadState(data.SceneManagerData);
        }

        private string BuildPath(string saveSlotName)
        {
            string directory = ProjectSettings.GlobalizePath(_saveLocation);
            return Path.Combine(directory, $"{saveSlotName}.{_serializer.FileExtension}");
        }

        private static void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static ISaveSerializer CreateSerializer(SaveBackend backend)
        {
            switch (backend)
            {
                case SaveBackend.Mock:
                    return new MockSerializer();
                case SaveBackend.Json:
                    return new JsonSaveSerializer();
                case SaveBackend.Binary:
                    return new BinarySaveSerializer();
                case SaveBackend.None:
                default:
                    return null;
            }
        }

        #endregion Internal functionality
    }
}
