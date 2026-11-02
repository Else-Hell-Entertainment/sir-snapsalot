using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public abstract partial class Trap : Node3D, IActivateable
    {
        #region IActivateable

        public abstract bool Activate();

        #endregion IActivateable
    }
}
