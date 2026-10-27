// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using EHE.SirSnapsalot.Managers;
using Godot;

namespace EHE.SirSnapsalot.GameStates
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
