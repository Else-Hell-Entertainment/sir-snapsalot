using System;
using EHE.Global.FSM;
using EHE.Global.Logging;
using Godot;

namespace EHE.UI.Menu
{
    /// <summary>
    /// Represents a state in the menu system to be used with the class-based <see cref="StateMachine"/>. This state is
    /// responsible for loading and managing a specific menu scene built in Godot. When creating a scene,
    /// it must inherit from <see cref="MenuScene"/> to be used correctly with this state.
    /// </summary>
    public class MenuState : IState
    {
        public MenuState(MenuRoot menuRoot, StringName scenePath)
        {
            if (scenePath.IsEmpty)
            {
                throw new ArgumentException("Scene path cannot be empty when creating menu state.");
            }

            _scenePath = scenePath;
            _menuRoot = menuRoot;
        }

        private MenuRoot _menuRoot;
        private PackedScene _packedScene;
        private MenuScene _scene;
        private StringName _scenePath;

        /// <summary>
        /// Should the scene be loaded on top of the current scene instead of replacing it. This is useful for creating a
        /// menu tree like settings -> video settings -> resolution etc. If you also want to keep the menu scene visible
        /// when opening an additive state, set StayVisible to true.
        /// </summary>
        public bool IsAdditive { get; init; }

        /// <summary>
        /// Should the menu scene stay visible when opening an additive state on top of it. State needs to be additive
        /// for this to work.
        /// </summary>
        public bool StayVisible { get; init; }

        public void Enter()
        {
            if (_scene != null)
            {
                _scene.ProcessMode = Node.ProcessModeEnum.Always;
                _scene.SetFocusMode(Control.FocusModeEnum.All);
                _scene.GrabFocus();
                return;
            }

            if (_packedScene == null)
            {
                _packedScene = GD.Load<PackedScene>(_scenePath);
                if (_packedScene == null)
                {
                    GD.PrintErr($"Couldn't load scene '{_scenePath}'!");
                    return;
                }
            }

            var scene = _packedScene.Instantiate();
            if (scene == null)
            {
                GD.PrintErr($"Couldn't instantiate scene from '{_scenePath}'!");
                return;
            }

            if (scene is not MenuScene menuScene)
            {
                GD.PrintErr($"Scene '{scene.Name}' does not inherit from MenuScene!");
                scene.QueueFree();
                return;
            }

            _scene = menuScene;
            SubscribeToSceneEvents();
            _menuRoot.AddChild(_scene);
            _scene.SetFocusMode(Control.FocusModeEnum.All);
            _scene.GrabFocus();
        }

        public void Exit(bool keepLoaded = false)
        {
            if (_scene != null)
            {
                if (keepLoaded && StayVisible)
                {
                    _scene.ProcessMode = Node.ProcessModeEnum.Disabled;
                    return;
                }

                UnsubscribeFromSceneEvents();
                this.LogDebug($"Exiting menu state: {_scene.Name}");
                _scene.QueueFree();
            }

            _scene = null;
            _packedScene = null;
        }

        private void SubscribeToSceneEvents()
        {
            this.LogDebug($"Subscribing to events for menu scene: {_scene.Name}");
            if (_scene != null)
            {
                _scene.RequestTransitionEvent += OnMenuSceneRequestTransition;
                _scene.ReturnToPreviousEvent += OnReturnToPreviousEvent;
            }
        }

        private void UnsubscribeFromSceneEvents()
        {
            this.LogDebug($"Unsubscribing from events for menu scene: {_scene.Name}");
            if (_scene != null)
            {
                _scene.RequestTransitionEvent -= OnMenuSceneRequestTransition;
                _scene.ReturnToPreviousEvent -= OnReturnToPreviousEvent;
            }
        }

        private void OnMenuSceneRequestTransition(string stateKey)
        {
            this.LogDebug($"Menu scene '{_scene.Name}' requested transition to state: {stateKey}");
            _menuRoot.RequestTransition(stateKey);
        }

        private void OnReturnToPreviousEvent()
        {
            this.LogDebug($"Menu scene '{_scene.Name}' requested return to previous state.");
            _menuRoot.ReturnToPrevious();
        }
    }
}
