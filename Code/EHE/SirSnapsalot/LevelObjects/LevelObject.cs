using EHE.Common.Godot.Logging;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public abstract partial class LevelObject : Node3D, IRotatable
    {
        private const float ROTATION_ANGLE_CW = -Mathf.Pi / 2;
        private const float ROTATION_ANGLE_CCW = Mathf.Pi / 2;

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
    }
}
