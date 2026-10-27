// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.FSM;
using EHE.SirSnapsalot.Managers;
using Godot;

namespace EHE.SirSnapsalot.GameStates
{
    public partial class GameStateBase : Node, IState
    {
        public virtual void Enter()
        {
            GameManager.Instance.AddChild(this);
            this.LogDebug($"Entering {Name} state!");
        }

        public virtual void Exit(bool keepLoaded = false)
        {
            GameManager.Instance.RemoveChild(this);
            this.LogDebug($"Exiting {Name} state!");
        }
    }
}
