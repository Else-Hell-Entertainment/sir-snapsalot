using System;
using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.CharacterSystem;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public partial class PressurePlate : Node3D
    {
        [Export]
        public Node3D Model { get; private set; } = null;

        [Export]
        public Area3D TriggerArea { get; private set; } = null;

        public override void _Ready()
        {
            if (!IsInstanceValid(TriggerArea))
            {
                this.LogError($"Cannot find {nameof(TriggerArea)}!");
                return;
            }

            TriggerArea.BodyEntered += OnTriggerAreaBodyEntered;
        }

        private void OnTriggerAreaBodyEntered(Node3D body)
        {
            if (body is Character character)
            {
                GD.Print($"{character.GetPath()} triggered {GetPath()}");
            }
        }
    }
}
