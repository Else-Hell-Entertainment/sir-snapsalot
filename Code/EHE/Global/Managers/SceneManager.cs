using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EHE.Global.Config;
using EHE.Global.Logging;
using EHE.Global.SaveSystem;
using EHE.Global.SceneManagement;
using Godot;
using Array = Godot.Collections.Array;

//using EHE.Scenes.PlayableScenes.Levels;

namespace EHE.Global.Managers
{
    public partial class SceneManager : Node, ISaveable<SceneManagerData>
    {
        #region Singleton

        public static SceneManager Instance { get; private set; }

        public SceneManager()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                GD.PrintErr("Duplicate instance of SceneManager! This should not happen.");
                QueueFree();
                return;
            }

            Initialize();
        }

        #endregion Singleton


        #region public properties

        public enum SceneTransitionType
        {
            Default,
            Instant,
            Fade,
            LoadingScreen,
        }

        #endregion public properties

        #region private housekeeping variables

        // The current playable scene is loaded under this root node.
        private Node _rootNode;

        // Reference to the currently active scene. It can be of any type. This node is used for internal tracking,
        // for other use cases, implement something specialized.
        private Node _currentScene;

        // Flag to keep internal track if a scene transition is currently underway so that the manager will not
        // accept any further requests.
        private bool _isTransitioning;

        #endregion private housekeeping variables

        #region SaveSystem

        public string CurrentSceneKey;

        public SceneCollection CurrentSceneCollection;

        #endregion SaveSystem

        #region Game specific properties

        /// <summary>
        /// Scene collection used in this particular game project as the default collection.
        /// </summary>
        private SceneCollections.Collections _defaultCollection = SceneCollections.Collections.LevelScenes;

        #endregion Game specific properties

        #region Loading Progress

        // Flag for waiting the threaded loading progress and checking the status of the loading process.
        // Also used to display progress bar.
        private bool _loadingInProgress;

        public Array LoadingProgress { get; private set; }

        // TaskCompletionSource to await the loading process. This is used to signal when the loading is complete.
        private TaskCompletionSource<bool> _loadingTcs;

        // Used for tracking loading progress.
        private string _loadingScenePath;

        #endregion Loading Progress


        private void Initialize()
        {
            Name = "SceneManager";
            _rootNode = new Node();
            _rootNode.Name = "RootNode";
            AddChild(_rootNode);
            LoadingProgress = new Array();
            CurrentSceneCollection = SceneCollections.GetSceneCollection(_defaultCollection);
        }

        public override void _Process(double delta)
        {
            CheckLoadingProgress();
            if (_loadingInProgress)
            {
                // Note that this skips the last frame as it's just a placeholder implementation.
                this.LogDebug("Loading progress: " + LoadingProgress);
            }
        }

        /// <summary>
        /// Request a scene transition with direct path to the scene file. This is not compatible with save system!
        /// Use this method only for one-shot scenes such as cutscenes or transitions which are not persistent.
        /// </summary>
        /// <param name="scenePath">Path to the scene file.</param>
        public void RequestSceneTransition(string scenePath)
        {
            SceneTransition sceneTransition = new SceneTransition(scenePath);
            RequestSceneTransition(sceneTransition);
        }

        /// <summary>
        /// Request the Scene Manager to transition from the currently active scene into the one defined by the
        /// SceneTransition data object. If a transition request is made while previous one is still in progress,
        /// it will be ignored.
        /// </summary>
        /// <param name="sceneTransition">Data object containing the desired scene transition.</param>
        public void RequestSceneTransition(SceneTransition sceneTransition)
        {
            this.LogDebug($"Requesting scene transition: {sceneTransition.TargetScenePath}");
            if (_isTransitioning)
            {
                this.LogInfo(
                    $"Scene transition called to {sceneTransition.TargetScenePath} but scene transition "
                        + $"already in progress."
                );

                return;
            }

            if (sceneTransition.TargetScenePath == null)
            {
                this.LogError("Target scene path is null. Cannot transition.");
                return;
            }

            if (!ResourceLoader.Exists(sceneTransition.TargetScenePath))
            {
                this.LogError($"Target scene not found at path: {sceneTransition.TargetScenePath}. Cannot transition.");

                return;
            }

            // Not awaiting here is on purpose. RequestTransition method is public to everything in the game, so it's
            // important that nothing accidentally awaits for it. Inside this manager, the execution is meant to be awaited,
            // but it needs to happen internally.
            BeginSceneTransition(sceneTransition);
        }

        private async Task BeginSceneTransition(SceneTransition sceneTransition)
        {
            this.LogDebug("Beginning scene transition: " + sceneTransition.TargetScenePath);
            _isTransitioning = true;

            await ExecuteTransition(sceneTransition);

            this.LogDebug("Scene transition finished.");
            _isTransitioning = false;
        }

        private async Task ExecuteTransition(SceneTransition sceneTransition)
        {
            // TODO: Handle transition type

            await UnloadCurrentScene();

            // Begin loading next level.
            await LoadNextSceneAsync(sceneTransition);

            await InitializeNextScene(sceneTransition);
        }

        /// <summary>
        /// Unloads the current scene in a controlled manner. If the scene is of type ILoadableScene, its UnloadScene
        /// method will be called and handled here. Otherwise, QueueFree is called directly.
        /// </summary>
        private async Task UnloadCurrentScene()
        {
            if (_currentScene == null)
            {
                this.LogInfo("No current scene loaded, nothing to unload.");
            }
            else
            {
                if (_currentScene is ILoadableScene scene)
                {
                    await scene.UnloadScene();
                }

                _currentScene.QueueFree();
                this.LogInfo("Unloaded scene from " + _currentScene.Name);
            }

            // Always wait for a single frame so that the old level is completely removed from the tree before moving on.
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        private async Task LoadNextSceneAsync(SceneTransition transition)
        {
            this.LogInfo("Loading next scene from " + transition.TargetScenePath);
            _loadingTcs = new TaskCompletionSource<bool>();
            _loadingScenePath = transition.TargetScenePath;
            ResourceLoader.LoadThreadedRequest(_loadingScenePath);
            _loadingInProgress = true;
            await _loadingTcs.Task;
        }

        private async Task InitializeNextScene(SceneTransition transition)
        {
            this.LogDebug("Initializing next scene from " + _loadingScenePath);
            var res = ResourceLoader.LoadThreadedGet(_loadingScenePath) as PackedScene;
            if (res == null)
            {
                this.LogFatalError(
                    "Could not access resource " + _loadingScenePath + " with LoadThreadedGet. Scene not loaded!"
                );

                return;
            }

            var nextScene = res.Instantiate();

            if (nextScene == null)
            {
                this.LogFatalError($"Could not instantiate scene from {_loadingScenePath}. Scene not loaded!");

                return;
            }

            _currentScene = nextScene;
            _rootNode.AddChild(_currentScene);
            this.LogDebug("New scene added to tree.");
            CurrentSceneKey = transition.SceneKey;
            if (_currentScene is ILoadableScene scene)
            {
                if (transition.Payload != null)
                {
                    this.LogDebug("Payload received for scene transition: " + transition.Payload);
                    scene.ReceiveSceneTransitionPayload(transition.Payload);
                }

                await scene.LoadScene();
            }

            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        private void CheckLoadingProgress()
        {
            if (!_loadingInProgress || _loadingTcs == null)
                return;

            var status = ResourceLoader.LoadThreadedGetStatus(_loadingScenePath, LoadingProgress);

            if (status == ResourceLoader.ThreadLoadStatus.Loaded)
            {
                _loadingInProgress = false;
                _loadingTcs.TrySetResult(true);
            }
            else if (status == ResourceLoader.ThreadLoadStatus.Failed)
            {
                _loadingInProgress = false;
                _loadingTcs.TrySetException(new Exception($"Failed to load {_loadingScenePath}"));
            }
        }

        /// <summary>
        /// Collects the entire save state of all scenes and other data relating to SceneManager and returns that as a data
        /// object ready to be written to file.
        /// </summary>
        /// <returns></returns>
        public SceneManagerData SaveState()
        {
            // TODO: Implement saving of scene states if needed. For now, just return an empty data object.
            SceneManagerData data = new SceneManagerData();
            return data;
        }

        /// <summary>
        /// Loads the entire state of the SceneManager, including all saved scene states into memory. All data already
        /// in memory is lost. Use this when the player uses load game function, NOT when switching to another scene
        /// with a state loaded into memory.
        /// </summary>
        /// <param name="state"></param>
        public void LoadState(SceneManagerData state) { }

        public void StartLoadedGame()
        {
            SceneTransition transition = new SceneTransition(SceneCollections.Collections.LevelScenes, CurrentSceneKey);

            RequestSceneTransition(transition);
        }
    }
}
