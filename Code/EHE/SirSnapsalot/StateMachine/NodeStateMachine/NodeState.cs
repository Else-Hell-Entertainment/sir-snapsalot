// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using Godot;

namespace EHE.SirSnapsalot.FSM
{
    public abstract partial class NodeState : Node3D
    {
        public abstract void Enter();
        public abstract void Exit();
    }
}
