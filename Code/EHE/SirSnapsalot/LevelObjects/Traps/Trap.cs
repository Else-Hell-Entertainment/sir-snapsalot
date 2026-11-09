using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public abstract partial class Trap : LevelObject, IActivatable
    {
        #region IActivatable

        public abstract bool Activate();

        #endregion IActivatable
    }
}
