// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Miska Rihu <miska.rihu@tuni.fi>

using EHE.SirSnapsalot.CharacterSystem;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public partial class PressurePlate : TrapActivator
    {
        protected override void OnTriggerAreaBodyEntered(Node3D body)
        {
            if (body is Character character)
            {
                ActivateConnectedTraps();
            }
        }
    }
}
