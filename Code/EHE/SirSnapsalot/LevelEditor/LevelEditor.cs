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
        /// This method also sanitizes the given level name by removing leading
        /// and trailing whitespace as well as other illegal characters
        /// (",", ".", "/").
        /// </remarks>
        private string BuildLevelFilePath(
            string levelName,
            string extension = DEFAULT_LEVEL_FILE_EXTENSION,
            string directory = DEFAULT_LEVEL_DIRECTORY_PATH
        )
        {
            // TODO: Refactor this to use regex.
            // TODO: Probably also make a separate saver/loader class to handle saving/loading level scenes.
            string sanitizedLevelName = levelName.Trim().Replace(" ", "_").Replace(".", "_").Replace("/", "_");
            return $"{directory}/{sanitizedLevelName}.{extension}";
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

        #endregion
    }
}
