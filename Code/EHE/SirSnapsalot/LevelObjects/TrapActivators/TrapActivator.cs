// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Miska Rihu <miska.rihu@tuni.fi>

using System;
using System.Collections.Generic;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public abstract partial class TrapActivator : Node3D
    {
        private readonly HashSet<IActivatable> _activatables = new HashSet<IActivatable>();

        [Export]
        protected Node3D Model { get; set; } = null;

        /// <summary>
        /// Area that detects bodies entering the space of this activator.
        /// When a body enters the trigger area, the
        /// <see cref="OnTriggerAreaBodyEntered"/> method is executed.
        /// </summary>
        [Export]
        protected Area3D TriggerArea { get; set; } = null;

        #region Godot API

        /// <summary>
        /// Initializes the <see cref="TriggerArea"/> of the activator. To add
        /// extra functionality, override the <see cref="OnReady()"/> method.
        /// </summary>
        /// <exception cref="InvalidOperationException">TriggerArea is null or the reference to it is invalid.</exception>
        public sealed override void _Ready()
        {
            if (!IsInstanceValid(TriggerArea))
            {
                throw new InvalidOperationException($"{nameof(TriggerArea)} is null or the reference is invalid! Check exports!");
            }

            TriggerArea.BodyEntered += OnTriggerAreaBodyEntered;

            OnReady();
        }

        #endregion


        #region Public API

        /// <summary>
        /// Adds the given <paramref name="activatable"/> from the connected objects.
        /// </summary>
        /// <param name="activatable">The <see cref="IActivatable"/> to connect.</param>
        /// <returns>
        /// <c>true</c> if the connection was added successfully,
        /// <c>false</c> otherwise (e.g., the target is already connected).
        /// </returns>
        public bool AddConnection(IActivatable activatable)
        {
            return _activatables.Add(activatable);
        }

        /// <summary>
        /// Removes the given <paramref name="activatable"/> from the connected objects.
        /// </summary>
        /// <param name="activatable">The <see cref="IActivatable"/> to disconnect.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="activatable"/> was successfully removed,
        /// <c>false</c> otherwise (e.g., if the activatable was never connected).
        /// </returns>
        public bool RemoveConnection(IActivatable activatable)
        {
            return _activatables.Remove(activatable);
        }

        /// <summary>
        /// Removes all connections.
        /// </summary>
        public void ClearConnections()
        {
            _activatables.Clear();
        }

        #endregion


        #region Protected API

        /// <summary>
        /// Execute when a body enters the <see cref="TriggerArea"/>.
        /// </summary>
        /// <param name="node">The body that entered the trigger area.</param>
        protected abstract void OnTriggerAreaBodyEntered(Node3D node);

        /// <summary>
        /// Executed after <see cref="_Ready()"/> for extra functionality.
        /// </summary>
        protected virtual void OnReady() { }

        /// <summary>
        /// Executes the <see cref="IActivatable.Activate()"/> for connected
        /// trap.
        /// </summary>
        protected virtual void ActivateConnectedTraps()
        {
            foreach (var activatable in _activatables)
            {
                activatable.Activate();
            }
        }

        #endregion
    }
}
