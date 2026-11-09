namespace EHE.SirSnapsalot
{
    public interface IRotatable
    {
        enum RotationDirection
        {
            None = 0,
            Clockwise,
            CounterClockwise
        }

        /// <summary>
        /// Rotates the object clockwise/counterclockwise.
        /// </summary>
        void Rotate(RotationDirection direction);
    }
}
