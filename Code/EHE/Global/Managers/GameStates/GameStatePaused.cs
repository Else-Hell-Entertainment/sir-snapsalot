using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.GameStates
{
    public partial class GameStatePaused : GameStateBase
    {
        public GameStatePaused()
        {
            Name = "GameStatePaused";
        }

        public bool IsAdditive => true;

        public override void Enter()
        {
            base.Enter();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GameManager.Instance.UIManager.UIEvents.RaisePauseGame();
            GameManager.Instance.UIManager.UIEvents.UnpauseGame += ResumeGame;
            GameManager.Instance.SceneTree.Paused = true;
        }

        public override void Exit(bool keepLoaded = false)
        {
            GameManager.Instance.UIManager.UIEvents.UnpauseGame -= ResumeGame;
            Input.MouseMode = Input.MouseModeEnum.Captured;
            base.Exit(keepLoaded);
        }

        private void ResumeGame()
        {
            this.LogDebug("Resuming game.");
            GameManager.Instance.SceneTree.Paused = false;
            GameManager.Instance.RequestStateTransition(GameState.Playing);
        }
    }
}
