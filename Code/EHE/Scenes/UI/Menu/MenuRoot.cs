using System.Collections.Generic;
using EHE.Global.Config;
using EHE.Global.FSM;
using EHE.Global.Logging;
using EHE.Global.SceneManagement;
using Godot;

namespace EHE.UI.Menu
{
    public abstract partial class MenuRoot : Control
    {
        [Export]
        private SceneCollections.Collections _sceneCollection;

        private StateMachine _stateMachine = new();

        private readonly Dictionary<string, IState> _menuStates = new();

        private SceneCollection _menuSceneCollection;

        #region Godot API

        public override void _Ready()
        {
            if (_sceneCollection == SceneCollections.Collections.None)
            {
                this.LogWarning("SceneCollection is not set in editor. Using default MenuScenes collection.");

                _menuSceneCollection = SceneCollections.GetSceneCollection(SceneCollections.Collections.MenuScenes);
            }
            else
            {
                _menuSceneCollection = SceneCollections.GetSceneCollection(_sceneCollection);
                this.LogDebug($"Using SceneCollection: {_sceneCollection}");
            }

            Initialize();
        }

        #endregion Godot API

        #region Public API

        /// <summary>
        /// Request a transition to a previously defined <c>MenuState</c> using the state key provided when the state was
        /// added to the state machine using <c>AddState</c> during <c>Initialize</c> call.
        /// </summary>
        /// <param name="newStateKey">Key used to identify the state. Defined in <c>Initialize.</c></param>
        public void RequestTransition(string newStateKey)
        {
            IState next = GetMenuState(newStateKey);
            this.LogDebug($"Requesting transition: {newStateKey}");
            if (next != null)
            {
                _stateMachine.ChangeState(next);
            }
            else
            {
                this.LogError(
                    $"Transition failed: State not found for key {newStateKey}. Make sure it's added to the "
                        + $"state machine in Initialize() using AddState."
                );
            }
        }

        /// <summary>
        /// Request a transition to the previous state in the state machine. This will transition to the last state that
        /// was active before the current state.
        /// </summary>
        public void ReturnToPrevious()
        {
            this.LogDebug("Requesting transition to previous state.");
            _stateMachine.TransitionToPrevious();
        }

        #endregion Public API


        #region Abstract Methods

        /// <summary>
        /// Called during <c>_Ready()</c> to initialize menu states and any other necessary setup.
        /// Use <c>AddState</c> to populate the state machine with menu states. Default usage is to use SceneCollection keys
        /// as state identifiers but custom scene paths can also be used if needed using method overload.
        /// </summary>
        protected abstract void Initialize();

        #endregion Abstract Methods


        #region Private Methods

        /// <summary>
        /// Adds a new state to the menu state machine. This method is intended for use with scene keys defined in the
        /// SceneCollections and will associate the state with the corresponding scene in the collection but can also be
        /// used with custom scene paths if needed.
        /// </summary>
        /// <param name="sceneKey"></param>
        /// <param name="isAdditive"></param>
        /// <param name="stayVisible"></param>
        protected void AddState(string sceneKey, bool isAdditive = false, bool stayVisible = false)
        {
            if (string.IsNullOrEmpty(sceneKey))
            {
                this.LogError("Scene key cannot be null or empty.");
                return;
            }

            string scenePath = _menuSceneCollection.GetScenePath(sceneKey);
            if (string.IsNullOrEmpty(scenePath))
            {
                this.LogError($"Scene path not found for key: {sceneKey}");
                return;
            }

            MenuState newState = new MenuState(this, scenePath) { IsAdditive = isAdditive, StayVisible = stayVisible };

            this.LogDebug(
                $"Adding state: {sceneKey} with path: {scenePath}, IsAdditive: {isAdditive}, StayVisible: {stayVisible}"
            );

            _menuStates[sceneKey] = newState;
        }

        protected void AddState(string stateKey, string scenePath, bool isAdditive = false, bool stayVisible = false)
        {
            if (string.IsNullOrEmpty(scenePath) || string.IsNullOrEmpty(stateKey))
            {
                this.LogError(
                    $"Attempting to add state: Scene path or key is null or empty for key {stateKey} and path {scenePath}"
                );

                return;
            }

            MenuState newState = new MenuState(this, scenePath) { IsAdditive = isAdditive, StayVisible = stayVisible };

            _menuStates[stateKey] = newState;
        }

        private IState GetMenuState(string key)
        {
            this.LogDebug($"Retrieving menu state for key: {key}");
            if (!_menuStates.TryGetValue(key, out IState state))
            {
                this.LogError($"State not found with key: {key}");
                return null;
            }

            return state;
        }

        #endregion Private Methods
    }
}
