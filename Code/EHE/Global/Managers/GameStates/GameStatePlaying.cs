using EHE.Global.Config;
using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.GameStates
{
    public partial class GameStatePlaying : GameStateBase
    {
        public GameStatePlaying()
        {
            Name = "GameStatePlaying";
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed(InputConfig.ESCAPE))
            {
                this.LogDebug("Escape pressed, pausing game...");
                GameManager.Instance.RequestStateTransition(GameState.Paused);
            }
        }
    }
}
