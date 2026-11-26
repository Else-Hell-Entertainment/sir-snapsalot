// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Miska Rihu <miska.rihu@tuni.fi>

namespace EHE.Common.Godot
{
    /// <summary>
    /// Defines placement behavior for an object.
    /// </summary>
    public interface IPlaceable
    {
        /// <summary>
        /// Whether this object can be placed in its current state.
        /// </summary>
        ///
        /// <remarks>
        /// Implementations should report placement constraints such as
        /// available position, state, or resources through this property.
        /// </remarks>
        bool CanPlace { get; }

        /// <summary>
        /// Whether this object has already been placed.
        /// </summary>
        bool IsPlaced { get; }

        /// <summary>
        /// Performs the object's placement logic.
        /// </summary>
        ///
        /// <remarks>
        /// Callers should check <see cref="CanPlace"/> before invoking this method.
        /// Implementations should update <see cref="IsPlaced"/> once placement completes.
        /// </remarks>
        void Place();
    }
}
