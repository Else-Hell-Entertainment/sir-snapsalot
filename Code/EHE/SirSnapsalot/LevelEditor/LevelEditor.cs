using Godot;
using System;
using System.Collections.Generic;
using EHE.Common.Godot.Extensions;
using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.LevelSystem;

namespace EHE.SirSnapsalot.LevelEditor
{
    public partial class LevelEditor : Node3D
    {
        /// <summary>
        /// The default directory path where level scenes are saved.
        /// </summary>
        private const string DEFAULT_LEVEL_DIRECTORY_PATH = "res://Scenes/Levels";

        /// <summary>
        /// The default file extension for level scenes.
        /// </summary>
        private const string DEFAULT_LEVEL_FILE_EXTENSION = "tscn";

        private HashSet<ComponentSelector> _componentSelectors = new HashSet<ComponentSelector>();
        private Level _level = null;

        [ExportGroup("Save UI")]
        [Export]
        private Button _saveAsButton;

        [Export]
        private LineEdit _levelNameEntry;

        [ExportGroup("Load UI")]
        [Export]
        private Button _loadButton;

        [Export]
        private LineEdit _levelNameLoadEntry;

        public override void _Ready()
        {
            // Find all ComponentSelectors in the UI and start listening to
            // their ComponentSelected event.
            foreach (var selector in this.GetChildren<ComponentSelector>(recurse: true))
            {
                // Caching the components allows unsubscribing from the events
                // later.
                if (!_componentSelectors.Add(selector))
                {
                    this.LogWarning("Trying to cache an already cached component selector!");
                    continue;
                }

                selector.ComponentSelected += OnComponentSelected;
            }

            // Cache reference to the Level node.
            _level = this.GetChild<Level>(recurse: true);
            if (!IsInstanceValid(_level))
            {
                throw new InvalidOperationException($"There is no level node or the reference is invalid.");
            }

            // Setup save UI.
            if (!IsInstanceValid(_saveAsButton) || !IsInstanceValid(_levelNameEntry))
            {
                throw new InvalidOperationException($"Missing references to save UI elements. Check exports!");
            }
            else
            {
                _saveAsButton.Pressed += OnSaveAsButtonPressed;
            }

            // Setup load UI.
            if (!IsInstanceValid(_loadButton) || !IsInstanceValid(_levelNameLoadEntry))
            {
                throw new InvalidOperationException($"Missing references to load UI elements. Check exports!");
            }
            else
            {
                _loadButton.Pressed += OnLoadButtonPressed;
            }
        }

        private void OnComponentSelected(CellComponent selectedComponent)
        {
            if (!IsInstanceValid(_level))
            {
                this.LogError($"There is no level node or the reference is invalid.");
                return;
            }

            _level.SetGhostComponent(selectedComponent);
        }

        #region Saving and loading level scenes

        /// <summary>
        /// Called when the <see cref="_saveAsButton"/> is pressed.
        /// Triggers saving the level with the name written in the
        /// <see cref="_levelNameEntry"/> field.
        /// </summary>
        private void OnSaveAsButtonPressed()
        {
            SaveLevel(_levelNameEntry.Text);
        }

        /// <summary>
        /// Called when the <see cref="_loadButton"/> is pressed.
        /// Triggers loading the level from the file with the name given in the
        /// <see cref="_levelNameLoadEntry"/> field.
        /// </summary>
        private void OnLoadButtonPressed()
        {
            LoadLevel(_levelNameLoadEntry.Text);
        }

        /// <summary>
        /// This method sanitizes the given level name by removing leading
        /// and trailing whitespace as well as other illegal characters
        /// (",", ".", "/").
        /// </summary>
        /// <param name="levelName">The level name to sanitize.</param>
        /// <returns>The sanitized level name.</returns>
        private string SanitizeLevelName(string levelName)
        {
            // TODO: Refactor this to use regex.
            // TODO: Probably also make a separate saver/loader class to handle saving/loading level scenes.
            return levelName.Trim().Replace(" ", "_").Replace(".", "_").Replace("/", "_");
        }

        /// <summary>
        /// Builds the full path to the level file with the given name.
        /// </summary>
        ///
        /// <param name="levelName">
        ///     The name of the level file without the file extension.</param>
        /// <param name="extension">
        ///     The file extension for the file.
        ///     Default is <see cref="DEFAULT_LEVEL_FILE_EXTENSION"/>.</param>
        /// <param name="directory">
        ///     The directory where the file should be saved.
        ///     Default is <see cref="DEFAULT_LEVEL_DIRECTORY_PATH"/>.</param>
        ///
        /// <returns>
        /// The full path to the level file with the given name. The
        /// </returns>
        ///
        /// <remarks>
        /// This method will automatically sanitize the given level using the
        /// <see cref="SanitizeLevelName"/> method on the <paramref name="levelName"/>
        /// parameter.
        /// </remarks>
        private string BuildLevelFilePath(
            string levelName,
            string extension = DEFAULT_LEVEL_FILE_EXTENSION,
            string directory = DEFAULT_LEVEL_DIRECTORY_PATH
        )
        {
            return $"{directory}/{SanitizeLevelName(levelName)}.{extension}";
        }

        /// <summary>
        /// Saves the current level node to a .tscn file with the given name.
        /// The files are saved to the directory defined by <see cref="DEFAULT_LEVEL_DIRECTORY_PATH"/>.
        /// </summary>
        ///
        /// <param name="levelName">
        ///     The name for the level file without file the file extension.</param>
        /// <param name="extension">
        ///     The file extension for the file.
        ///     Default is <see cref="DEFAULT_LEVEL_FILE_EXTENSION"/>.</param>
        /// <param name="directory">
        ///     The directory where the file should be saved.
        ///     Default is <see cref="DEFAULT_LEVEL_DIRECTORY_PATH"/>.</param>
        ///
        /// <returns>
        /// <c>true</c> if the file was saved successfully,
        /// <c>false</c> otherwise.
        /// </returns>
        private bool SaveLevel(
            string levelName,
            string extension = DEFAULT_LEVEL_FILE_EXTENSION,
            string directory = DEFAULT_LEVEL_DIRECTORY_PATH
        )
        {
            if (!IsInstanceValid(_level))
            {
                this.LogError($"There is no level to save.");
                return false;
            }

            PackedScene levelScene = new PackedScene();
            levelScene.Pack(_level);
            Error saveError = ResourceSaver.Save(levelScene, BuildLevelFilePath(levelName, extension, directory));

            if (saveError != Error.Ok)
            {
                this.LogError($"Failed to save level: {saveError}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads a level from a scene file and adds it to the scene tree.
        /// </summary>
        ///
        /// <param name="levelName">
        ///     The name of the scene file without the file extension.</param>
        /// <param name="extension">
        ///     The file extension of the file.
        ///     Default is <see cref="DEFAULT_LEVEL_FILE_EXTENSION"/>.</param>
        /// <param name="directory">
        ///     The path to the directory where the scene file is located.
        ///     Default is <see cref="DEFAULT_LEVEL_DIRECTORY_PATH"/>.</param>
        ///
        /// <returns>
        /// <c>true</c> if the level was loaded successfully,
        /// <c>false</c> otherwise.
        /// </returns>
        private bool LoadLevel(
            string levelName,
            string extension = DEFAULT_LEVEL_FILE_EXTENSION,
            string directory = DEFAULT_LEVEL_DIRECTORY_PATH
        )
        {
            string sanitizedLevelName = SanitizeLevelName(levelName);

            if (sanitizedLevelName == "" || sanitizedLevelName.Length == 0)
            {
                this.LogError($"Failed to load level: No valid level name provided.");
                return false;
            }

            string loadPath = BuildLevelFilePath(sanitizedLevelName, extension, directory);

            if (!ResourceLoader.Exists(loadPath))
            {
                this.LogError($"Failed to load level: Invalid path '{loadPath}'.");
                return false;
            }

            PackedScene levelScene = ResourceLoader.Load<PackedScene>(loadPath);

            if (levelScene == null)
            {
                this.LogError($"Failed to load level from path '{loadPath}'. Is the path correct?");
                return false;
            }

            Level levelInstance = levelScene.Instantiate<Level>();

            if (levelInstance == null)
            {
                this.LogError($"Failed to load level: There is no valid level scene at path '{loadPath}'.");
                return false;
            }

            if (IsInstanceValid(_level))
            {
                // Renaming the current level prevents Godot from overriding
                // the name given to the new node in case the new name is the
                // same as the old one. For example, if the current level was
                // TestLevel1 and you loaded a file named TestLevel1, Godot
                // would rename the node to TestLevel2 since the name was
                // already in use.
                _level.Name = "removed_level";
                _level.QueueFree();
                _level = null;
            }

            _level = levelInstance;
            AddChild(_level);
            _level.Name = $"{sanitizedLevelName}";
            return true;
        }

        #endregion
    }
}
