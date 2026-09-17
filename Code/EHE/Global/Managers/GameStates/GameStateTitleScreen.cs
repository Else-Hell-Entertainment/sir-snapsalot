using EHE.Global.Managers;
using Godot;

namespace EHE.Global.GameStates
{
    public partial class GameStateTitleScreen : GameStateBase
    {
        public GameStateTitleScreen()
        {
            Name = "GameStateTitleScreen";
        }

        public override void Enter()
        {
            base.Enter();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            PlayerManager.Instance.DestroyPlayer();
        }

        public override void Exit(bool keepLoaded = false)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            base.Exit(keepLoaded);
        }
    }
}
