using System;
using System.Collections.Generic;
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

        [Export]
        public Area3D ConnectionDetectors { get; private set; } = null;

        private List<IActivatable> _activationTargets = new List<IActivatable>();

        public override void _Ready()
        {
            if (!IsInstanceValid(TriggerArea))
            {
                this.LogError($"Cannot find {nameof(TriggerArea)}!");
                return;
            }

            TriggerArea.BodyEntered += OnTriggerAreaBodyEntered;

            if (!IsInstanceValid(ConnectionDetectors))
            {
                this.LogError($"Cannot find {nameof(ConnectionDetectors)}!");
                return;
            }

            ConnectionDetectors.BodyEntered += OnConnectionDetectorsBodyEntered;
        }

        private void OnTriggerAreaBodyEntered(Node3D body)
        {
            if (body is Character character)
            {
                GD.Print($"{character.GetPath()} triggered {GetPath()}");
                ActivateConnectedTraps();
            }
        }

        /// <summary>
        /// Detects when a <paramref name="body"> enters the
        /// <see cref="ConnectionDetectors"/> area and adds it to the list of
        /// activation targets if it is <see cref="IActivatable"/>.
        /// </summary>
        /// <param name="body"></param>
        private void OnConnectionDetectorsBodyEntered(Node3D body)
        {
            // TODO: Only do this when trap has been actually placed.
            // Hovering the trap next to the pressure plate shouldn't make a
            // connection!
            if (body is IActivatable activateable)
            {
                AddActivationTarget(activateable);
            }
        }

        /// <summary>
        /// Activates each target in the <see cref="_activationTargets"/> list.
        /// </summary>
        private void ActivateConnectedTraps()
        {
            foreach (IActivatable activateable in _activationTargets)
            {
                activateable.Activate();
            }
        }

        /// <summary>
        /// Adds the given <paramref name="activatable"/> to the
        /// <see cref="_activationTargets"/> list.
        /// </summary>
        /// <param name="activatable"></param>
        private void AddActivationTarget(IActivatable activatable)
        {
            // TODO: Make sure the thing has already been placed before connecting it!
            this.LogInfo($"Added {activatable.GetType()} to activation targets.");
            _activationTargets.Add(activatable);
        }
    }
}
