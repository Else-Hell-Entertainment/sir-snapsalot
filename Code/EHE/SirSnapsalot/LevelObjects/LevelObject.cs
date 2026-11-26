using EHE.Common.Godot;
using EHE.Common.Godot.Logging;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public abstract partial class LevelObject : Node3D, IRotatable, IPlaceable
    {
        private const float ROTATION_ANGLE_CW = -Mathf.Pi / 2;
        private const float ROTATION_ANGLE_CCW = Mathf.Pi / 2;

        [Export]
        private CollisionShape3D _collisionShape = null;

        // TODO: Implement placement restrictions for LevelObjects here.
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        ///
        /// <remarks>
        /// Still WIP. Currently, this will always return <c>true</c>.
        /// </remarks>
        public bool CanPlace { get; } = true;

        /// <inheritdoc/>
        public bool IsPlaced { get; private set; } = false;

        #region Godot API

        /// <summary>
        /// Checks if a <see cref="CollisionShape3D"/> is defined for this
        /// object and sets its <see cref="CollisionShape3D.Disabled"/>
        /// property to <c>true</c>.
        /// </summary>
        ///
        /// <seealso cref="Node._Ready()"/>
        public override void _Ready()
        {
            if (_collisionShape == null)
            {
                this.LogInfo($"No collision shape defined, ignoring.");
            }
            else
            {
                _collisionShape.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
            }
        }

        #endregion

        #region IRotatable

        /// <inheritdoc/>
        public void Rotate(IRotatable.RotationDirection direction)
        {
            this.LogDebug($"Rotating, {direction}");
            switch (direction)
            {
                case IRotatable.RotationDirection.None:
                    // Nothing to do.
                    this.LogWarning($"Rotation direction was {IRotatable.RotationDirection.None}. Was this intentional?");
                    break;
                case IRotatable.RotationDirection.Clockwise:
                    GlobalRotate(Vector3.Up, ROTATION_ANGLE_CW);
                    break;
                case IRotatable.RotationDirection.CounterClockwise:
                    GlobalRotate(Vector3.Up, ROTATION_ANGLE_CCW);
                    break;
                default:
                    this.LogError("Invalid rotation direction!");
                    break;
            }
        }

        #endregion

        #region IPlaceable

        /// <summary>
        /// Enables the collision shape for this level object and sets
        /// <see cref="IsPlaced"/> to <c>true</c>.
        /// </summary>
        ///
        /// <remarks>
        /// If extra logic needed (for example custom animations, etc.), that
        /// can be done by overriding the <see cref="OnPlace()"/> method.
        /// </remarks>
        public void Place()
        {
            if (IsInstanceValid(_collisionShape))
            {
                _collisionShape.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
            }

            IsPlaced = true;
            OnPlace();
        }

        #endregion

        #region Protected API

        /// <summary>
        /// Executed after the common logic in the <see cref="Place()"/> method.
        /// </summary>
        protected virtual void OnPlace() { }

        #endregion
    }
}
