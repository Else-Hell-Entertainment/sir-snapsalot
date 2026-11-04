using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public abstract partial class Trap : Node3D, IActivatable
    {
        #region IActivatable

        public abstract bool Activate();

        #endregion IActivatable
    }
}
