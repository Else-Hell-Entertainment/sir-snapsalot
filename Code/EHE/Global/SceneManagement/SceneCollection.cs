using EHE.Global.Logging;
using Godot;
using Godot.Collections;

namespace EHE.Global.SceneManagement
{
    [Tool]
    [GlobalClass]
    public partial class SceneCollection : Resource
    {
        [Export]
        public string CollectionName;

        [Export]
        private string _addNewSceneKey;

        [Export]
        private string _addNewSceneUid;

        [ExportToolButton("Add new scene to collection")]
        private Callable AddSceneButton => Callable.From(AddScene);

        [Export]
        private Dictionary<string, string> _savedScenePaths;

        [ExportGroup("Automatic collection management.")]
        [Export]
        private string _removeSceneWithKey;

        [ExportToolButton("Remove scene from collection.")]
        private Callable RemoveSceneButton => Callable.From(RemoveScene);

        [ExportToolButton("Validate and update collection entries")]
        private Callable UpdatePathsButton => Callable.From(UpdatePaths);

        private Godot.Collections.Dictionary<string, string> _sceneUidDict = new();

        private Godot.Collections.Dictionary<string, string> _scenePathDict = new();

        #region Public API

        /// <summary>
        /// Returns the scene path for a given key. The key is defined in the collection and can be used to reference
        /// the scene in code or in exported properties. If the key is not found, an error is logged and null is returned.
        /// </summary>
        /// <param name="scenePathKey">Unique identifier key which was used to save the scene.</param>
        /// <returns>Path to the scene identified by the key.</returns>
        public string GetScenePath(string scenePathKey)
        {
            if (_scenePathDict.TryGetValue(scenePathKey, out var path))
            {
                return path;
            }

            this.LogFatalError(
                $"Scene path not found for key: {scenePathKey} in collection: {CollectionName}. "
                    + "Check that the key is correct and that the scene has been added to the collection."
            );

            return null;
        }

        /// <summary>
        /// Returns the scene uid for a given key. The key is defined in the collection and can be used to reference
        /// the scene in code or in exported properties. If the key is not found, an error is logged and null is returned.
        /// </summary>
        /// <param name="scenePathKey">Unique identifier key which was used to save the scene.</param>
        /// <returns>Uid in string format of the scene identified by the key.</returns>
        public string GetSceneUid(string scenePathKey)
        {
            if (_sceneUidDict.TryGetValue(scenePathKey, out var uid))
            {
                return uid;
            }

            this.LogFatalError(
                $"Scene UID not found for key: {scenePathKey} in collection: {CollectionName}. "
                    + "Check that the key is correct and that the scene has been added to the collection."
            );

            return null;
        }

        /// <summary>
        /// Returns all keys in the collection as an array of strings. These keys can be used to reference the scenes in
        /// code or in exported properties.
        /// </summary>
        /// <returns>All keys as <c>strings</c> in array.</returns>
        public string[] GetKeys()
        {
            string[] keys = new string[_sceneUidDict.Keys.Count];
            _sceneUidDict.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Returns all keys in collection as a string formatted for use in the hint_string of an exported property.
        /// </summary>
        /// <returns>All keys in one string separated by commas.</returns>
        public string GetKeysAsString()
        {
            string keys = "";

            foreach (var key in _sceneUidDict.Keys)
            {
                keys += key + ",";
            }

            keys = keys.TrimEnd(',');
            return keys;
        }

        #endregion public API

        #region Private Methods

        /// <summary>
        /// Adds a scene to the collection. Grabs the values from input fields populated in the editor, validates them
        /// and adds to both UID collection and the scene path collection using the same key as identifier.
        /// </summary>
        private void AddScene()
        {
            if (!ValidateSceneInput() || !ValidateUid(_addNewSceneUid))
            {
                return;
            }

            var uid = ResourceUid.TextToId(_addNewSceneUid);
            string path = ResourceUid.GetIdPath(uid);

            foreach (var value in _scenePathDict.Values)
            {
                if (value.Equals(path))
                {
                    GD.PrintErr(
                        $"Duplicate scene path '{path}' found in collection for uid: {_addNewSceneUid}. "
                            + "This can happen if a scene was deleted and recreated with the same path or due to issues with "
                            + "importing or version control synchronization. Check the entries and scene files manually."
                    );

                    return;
                }
            }

            _sceneUidDict.Add(_addNewSceneKey, _addNewSceneUid);
            _scenePathDict.Add(_addNewSceneKey, path);

            GD.Print($"Scene added to collection. Key : {_addNewSceneKey}, UID: {_addNewSceneUid}, Path : {path} ");

            _addNewSceneKey = string.Empty;
            _addNewSceneUid = string.Empty;
            NotifyPropertyListChanged();
        }

        private void RemoveScene()
        {
            if (!_sceneUidDict.ContainsKey(_removeSceneWithKey) && !_scenePathDict.ContainsKey(_removeSceneWithKey))
            {
                GD.Print($"No scenes found in collection with key {_removeSceneWithKey}. Nothing to remove.");
            }
            else
            {
                _sceneUidDict.Remove(_removeSceneWithKey);
                _scenePathDict.Remove(_removeSceneWithKey);
                GD.Print($"Scene with key {_removeSceneWithKey} removed from collection.");
                _removeSceneWithKey = string.Empty;
                NotifyPropertyListChanged();
            }
        }

        /// <summary>
        /// Ensures that the input in the scene key and UID fields is valid before allowing a new scene to be added to the
        /// collection. The fields cannot be empty and the key and UID must be unique within the collection.
        /// Checks only that the input is formally correct and not duplicate, not the validity of the uid itself.
        /// Use <see cref="ValidateUid"/> to ensure the uid is valid and hasn't been corrupted.
        /// </summary>
        /// <returns><c>true</c> if both inputs are valid, <c>false</c> otherwise.</returns>
        private bool ValidateSceneInput()
        {
            if (string.IsNullOrEmpty(_addNewSceneKey) || string.IsNullOrEmpty(_addNewSceneUid))
            {
                GD.PrintErr("Scene key and UID must be set before adding to collection.");
                return false;
            }

            if (_sceneUidDict.ContainsKey(_addNewSceneKey) || _scenePathDict.ContainsKey(_addNewSceneKey))
            {
                GD.PrintErr($"Scene key '{_addNewSceneKey}' already exists in collection. Please use a unique key.");

                return false;
            }

            foreach (var value in _sceneUidDict.Values)
            {
                if (value == _addNewSceneUid)
                {
                    GD.PrintErr(
                        $"Scene UID '{_addNewSceneUid}' already exists in collection. Please use a unique UID."
                    );

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks that the provided UID is valid and corresponds to an existing resource which has been registered with
        /// the ResourceUid system. Will not attempt to automatically recover as it's safer to establish the root cause
        /// for why the uid is invalid.
        /// </summary>
        /// <param name="uid">Uid in string format.</param>
        /// <returns><c>true</c> if the uid is valid and registered, <c>false</c> otherwise.</returns>
        private bool ValidateUid(string uid)
        {
            var uidValue = ResourceUid.TextToId(uid);

            if (uidValue == ResourceUid.InvalidId || !ResourceUid.HasId(uidValue))
            {
                GD.PrintErr(
                    $"Scene uid {uid} is invalid. The scene may have been re-imported, the uid file removed "
                        + $"or the uid was lost in version control."
                );

                return false;
            }

            return true;
        }

        private void UpdatePaths()
        {
            GD.Print("Updating paths.");
            foreach (var entry in _sceneUidDict)
            {
                string key = entry.Key;
                string uid = entry.Value;

                GD.Print($"Checking key {key} and uid {uid}.");

                // All cases that need to be checked:
                //
                // Normal issues (can occur as result of normal editor work):
                //
                // 1. Uid is invalid, but there is a valid entry in scene paths. Uid has been corrupted and needs to update.
                //    This can happen if the uid file got deleted or corrupted, but the scene file is still there and has
                //    a valid path.
                //
                // 2. Uid is valid, but the scene path is stale and needs to update. Scene was probably just moved.
                //
                // Problematic issues which technically should not happen and may indicate a bug or user error.
                //
                // 3. Uid is invalid and the scene path or the entire entry in the path dictionary is also missing or
                //    corrupted so only the key exists anymore with broken values on both dictionaries.
                //    Something messed with the dictionaries and should be manually checked.
                //
                // 4. Uid is valid but scene path entry is missing entirely. Add the missing entry.
                //    This can be automatically fixed but the path dictionary should update automatically. This is more
                //    likely a user error than a bug but should still be monitored.
                //
                // 5. There are entries in the path dictionary but no corresponding entry in uid dictionary. Similar to 4.

                // Check for case 1 & 3 first.
                if (!ValidateUid(uid))
                {
                    GD.PushWarning(
                        $"Invalid Uid {uid} found in collection {CollectionName} with key {key}. Attempting to fix."
                    );

                    if (!_scenePathDict.ContainsKey(key)) // This is case 3. Uid is invalid and no entry at all in paths.
                    {
                        GD.PrintErr(
                            $"No path found for key {key} in collection {CollectionName}. Cannot fix invalid Uid {uid}. "
                                + $"Fix broken entry manually."
                        );

                        continue;
                    }

                    string path = _scenePathDict[key];
                    var newUid = ResourceUid.PathToUid(path);
                    if (ValidateUid(newUid))
                    {
                        // This is case 1 and will probably happen occasionally.
                        _sceneUidDict[key] = newUid;
                        GD.PushWarning(
                            $"Successfully updated Uid for key {key} in collection {CollectionName}. "
                                + $"Old Uid: {uid}, new Uid: {newUid}."
                        );
                    }
                    else
                    {
                        // This is a variation of case 3. Original uid was invalid, scene exists in paths, but it has a
                        // broken uid for some reason. The entries are broken in multiple places and should be checked.
                        GD.PrintErr(
                            $"Failed to update Uid for key {key} in collection {CollectionName}. "
                                + $"Path {path} exists but has no uid and might be corrupted. Manual fix is required."
                        );
                    }

                    continue;
                }

                string correctPath = ResourceUid.UidToPath(uid);
                // This is case 2 which is a non-issue. Someone moved the scene and that's why this method exists.
                if (_scenePathDict.TryGetValue(key, out var scenePath))
                {
                    if (correctPath != scenePath)
                    {
                        GD.PushWarning(
                            $"Found stale file path {scenePath} in collection {CollectionName}. Updating with"
                                + $" fresh path {correctPath} for key {key}."
                        );

                        _scenePathDict[key] = correctPath;
                    }
                }
                // This is case 4. Not critical, can be fixed but shouldn't normally happen.
                else
                {
                    _scenePathDict.Add(key, correctPath);
                    GD.PushWarning(
                        $"Collection {CollectionName} scene path dictionary was missing an entry "
                            + $"corresponding a valid uid entry with key {key} and uid {uid}. "
                            + $"Added missing entry with path {correctPath} based on the valid uid."
                    );
                }
            }

            foreach (var entry in _scenePathDict)
            {
                string key = entry.Key;
                GD.Print($"Checking key {key} and path {entry.Value}.");
                // Since the whole uid dictionary was already checked, this will just catch any missing entries (case 5).
                if (!_sceneUidDict.ContainsKey(key))
                {
                    GD.PushWarning(
                        $"Found entry in scene path dictionary with key {key} but no corresponding "
                            + $"entry in uid dictionary. Attempting to fix."
                    );

                    string path = _scenePathDict[key];
                    var newUid = ResourceUid.PathToUid(path);
                    if (ValidateUid(newUid))
                    {
                        _sceneUidDict[key] = newUid;
                        GD.PushWarning($"New Uid {newUid} created for key {key} in collection {CollectionName}.");
                    }
                    else
                    {
                        GD.PrintErr(
                            $"Failed to create Uid for key {key} in collection {CollectionName} based on path {path}. "
                                + $"The path might be corrupted or the scene file might be missing. Manual fix is required."
                        );
                    }
                }
            }

            NotifyPropertyListChanged();
        }

        #endregion Private Methods

        #region [Tool] Method Overrides

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties = [];

            // Left as an option to enable the property here.
            // properties.Add(
            //     new Dictionary()
            //     {
            //         { "name", "SavedScenes (Read only)" },
            //         { "type", (int)Variant.Type.Dictionary },
            //         { "usage", (int)PropertyUsageFlags.ReadOnly | (int)PropertyUsageFlags.Default },
            //     }
            // );

            properties.Add(
                new Dictionary()
                {
                    { "name", "Manual collection management. Use with caution!" },
                    { "type", (int)Variant.Type.Nil },
                    { "hint_string", "management_" },
                    { "usage", (int)PropertyUsageFlags.Group | (int)PropertyUsageFlags.Default },
                }
            );

            properties.Add(
                new Dictionary()
                {
                    { "name", "management_ScenePathDictionary" },
                    { "type", (int)Variant.Type.Dictionary },
                    { "hint", (int)PropertyHint.DictionaryType },
                    { "hint_string", "String;String" },
                    { "usage", (int)PropertyUsageFlags.Default },
                }
            );

            properties.Add(
                new Dictionary()
                {
                    { "name", "management_SceneUidDictionary" },
                    { "type", (int)Variant.Type.Dictionary },
                    { "hint", (int)PropertyHint.DictionaryType },
                    { "hint_string", "String;String" },
                    { "usage", (int)PropertyUsageFlags.Default },
                }
            );

            return properties;
        }

        public override Variant _Get(StringName property)
        {
            // If defined in _GetPropertyList, this needs to be enabled. When using [Export] annotation, _Get and _Set
            // are not called on that property!
            //
            // if (property == "SavedScenes (Read only)")
            // {
            //     Godot.Collections.Dictionary<string, string> savedPaths = new();
            //
            //     foreach (var entry in _scenePathDict)
            //     {
            //         savedPaths.Add(entry.Key, entry.Value);
            //     }
            //
            //     return savedPaths;
            // }

            if (property == "management_ScenePathDictionary")
            {
                return _scenePathDict;
            }

            if (property == "management_SceneUidDictionary")
            {
                return _sceneUidDict;
            }

            return default;
        }

        public override bool _Set(StringName property, Variant value)
        {
            if (property == "management_ScenePathDictionary")
            {
                _scenePathDict = value.As<Godot.Collections.Dictionary<string, string>>();
                NotifyPropertyListChanged();
                return true;
            }

            if (property == "management_SceneUidDictionary")
            {
                _sceneUidDict = value.As<Godot.Collections.Dictionary<string, string>>();
                NotifyPropertyListChanged();
                return true;
            }

            return false;
        }

        public override void _ValidateProperty(Dictionary property)
        {
            if (property["name"].AsStringName() == PropertyName._savedScenePaths)
            {
                Godot.Collections.Dictionary<string, string> temp = new();
                foreach (var entry in _scenePathDict)
                {
                    temp.Add(entry.Key, entry.Value);
                }

                _savedScenePaths = temp;
                property["usage"] = (int)PropertyUsageFlags.ReadOnly | (int)PropertyUsageFlags.Default;
            }
        }

        #endregion [Tool] Method Overrides
    }
}
