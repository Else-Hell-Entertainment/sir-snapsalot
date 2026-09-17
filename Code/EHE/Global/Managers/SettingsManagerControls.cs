using EHE.Global.Logging;
using Godot;

namespace EHE.Global.Managers
{
    public partial class SettingsManager
    {
        public void ApplyControlSettings()
        {
            foreach (var action in Settings.ControlSettings.InputActionMap)
            {
                if (InputMap.HasAction(action.Key))
                {
                    RemapAction(action.Key, action.Value);
                }
                else
                {
                    this.LogError(
                        $"Action '{action.Key}' does not exist in InputMap. Cannot apply InputEvent. "
                            + $"Save file may be corrupted."
                    );
                }
            }

            { }
        }

        public void RemapAction(string actionName, InputEvent newEvent)
        {
            if (InputMap.HasAction(actionName))
            {
                this.LogDebug($"Removing InputEvents for action: {actionName}");
                InputMap.ActionEraseEvents(actionName);
            }

            InputMap.ActionAddEvent(actionName, newEvent);
            this.LogDebug($"Adding InputEvent for action: {actionName}");

            if (InputMap.ActionHasEvent(actionName, newEvent))
            {
                this.LogInfo($"Remapped action: {actionName} to new event: {newEvent.AsText()}");
                Settings.ControlSettings.InputActionMap[actionName] = newEvent;
            }
            else
            {
                this.LogError($"Failed to add InputEvent {newEvent} for action: {actionName}");
            }
        }

        public void ResetInputDefaults()
        {
            this.LogInfo("Resetting input mapping to defaults");

            InputMap.LoadFromProjectSettings();
            Settings.ControlSettings.InputActionMap.Clear();
        }
    }
}
