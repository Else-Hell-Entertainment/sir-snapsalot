using EHE.Global.Logging;
using EHE.LevelSystem;
using Godot;

namespace SirSnapsalot.EHE.Global.Managers
{
    public partial class LevelManager : Node
    {
        #region Singleton

        public static LevelManager Instance { get; private set; }

        public LevelManager()
        {
            if (Instance == null)
            {
                Instance = this;
                Name = "Level Manager";
            }
            else if (Instance != this)
            {
                this.LogError("Duplicate instance of LevelManager! This should not happen.");
                QueueFree();
                return;
            }
            ProcessMode = ProcessModeEnum.Always;

            this.LogDebug("Instance created successfully.");

            CallDeferred(MethodName.Initialize);
        }

        private void Initialize() { }

        #endregion Singleton

        #region Properties

        public Level CurrentLevel { get; private set; }

        #endregion Properties


        public void ChangeLevel(Level level)
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.QueueFree();
            }
            CurrentLevel = level;
        }
    }
}
