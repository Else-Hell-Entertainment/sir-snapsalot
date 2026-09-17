using System.Collections.Generic;
using EHE.Global.Config;
using EHE.Global.Managers;
using Godot;

namespace EHE.UI.Menu
{
    public partial class ControlSettingsMenu : MenuScene
    {
        [Export]
        private Container _controlsContainer;

        [Export]
        private PackedScene _controlSettingButtonScene;

        [Export]
        private Panel _remapPromptPanel;

        [Export]
        private Label _actionLabel;

        private List<ControlSettingButton> _controlSettingButtons = new();

        private bool _isListeningForRemap = false;
        private string _currentRemapAction = string.Empty;

        public override void _Ready()
        {
            base._Ready();
            _remapPromptPanel.Hide();
            PopulateControls();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            foreach (var button in _controlSettingButtons)
            {
                button.OnRemapRequested -= OnRemapRequested;
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (_isListeningForRemap && @event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
            {
                SettingsManager.Instance.RemapAction(_currentRemapAction, @event);
                _isListeningForRemap = false;
                _currentRemapAction = string.Empty;
                RefreshControlTexts();
                AcceptEvent();
                _remapPromptPanel.Hide();
            }
        }

        private void PopulateControls()
        {
            List<string> inputActions = InputConfig.GetAllInputActions();
            for (int i = 0; i < inputActions.Count; i++)
            {
                string actionName = inputActions[i];
                ControlSettingButton controlButton = _controlSettingButtonScene.Instantiate<ControlSettingButton>();

                controlButton.SetInputAction(actionName);
                controlButton.OnRemapRequested += OnRemapRequested;
                _controlSettingButtons.Add(controlButton);
                _controlsContainer.AddChild(controlButton);
            }
        }

        private void RefreshControlTexts()
        {
            foreach (var button in _controlSettingButtons)
            {
                button.RefreshText();
            }
        }

        private void OnRemapRequested(string actionName)
        {
            _remapPromptPanel.Show();
            _actionLabel.Text = actionName;
            _currentRemapAction = actionName;
            _isListeningForRemap = true;
        }
    }
}
