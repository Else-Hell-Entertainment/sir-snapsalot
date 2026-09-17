using System;
using EHE.Global.Logging;
using Godot;

public partial class ControlSettingButton : Control
{
    /// <summary>
    /// Displayed to the player as the name of the input action (e.g., "Jump", "Shoot").
    /// May be different from the actual InputMap action name, which is used internally by Godot
    /// (e.g., "ui_jump", "ui_shoot").
    /// </summary>
    public string InputActionName;

    /// <summary>
    /// Which key or button is currently bound to this input action. Displayed to the player in menu
    /// and is pulled from the InputEvent.
    /// </summary>
    public string InputActionBinding;

    /// <summary>
    /// The actual InputMap action name used internally by Godot. This is the name that is used to check for
    /// input events in the game code.
    /// </summary>
    private string _inputActionName;

    [Export]
    private Button _remapButton;

    [Export]
    private Label _inputLabel;

    public event Action<string> OnRemapRequested;

    public override void _Ready()
    {
        base._Ready();
        _remapButton.Pressed += OnRemapButtonPressed;
    }

    private void OnRemapButtonPressed()
    {
        OnRemapRequested?.Invoke(_inputActionName);
    }

    public void RefreshText()
    {
        if (InputMap.ActionGetEvents(_inputActionName).Count < 1)
        {
            return;
        }
        _inputLabel.Text = InputMap.ActionGetEvents(_inputActionName)[0].AsText();
    }

    public void SetInputAction(string actionName)
    {
        _inputActionName = actionName;
        if (InputMap.HasAction(actionName) && InputMap.ActionGetEvents(actionName).Count > 0)
        {
            var inputEvent = InputMap.ActionGetEvents(actionName)[0];
            InputActionBinding = inputEvent.AsText();
            _inputLabel.Text = InputActionBinding;
            _remapButton.Text = actionName;
        }
        else
        {
            this.LogError($"Input action '{actionName}' does not exist in InputMap.");
        }
    }
}
