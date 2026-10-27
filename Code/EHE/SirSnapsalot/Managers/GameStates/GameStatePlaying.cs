// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.Config;
using EHE.SirSnapsalot.Managers;
using Godot;

namespace EHE.SirSnapsalot.GameStates
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
