using Godot;
using System;
using EHE.SirSnapsalot.LevelSystem;
using EHE.Common.Godot.Extensions;
using EHE.Common.Godot.Logging;

namespace EHE.SirSnapsalot.LevelEditor
{
    public partial class ComponentSelector : Control
    {
        public delegate void ComponentSelectedEventHandler(CellComponent component);
        public event ComponentSelectedEventHandler ComponentSelected;

        private Button _selectButton = null;
        private Label _counterLabel = null;

        /// <summary>
        /// Link to the cell component that clicking this button wil select.
        /// </summary>
        [Export]
        public CellComponent CellComponent { get; private set; } = null;

        public override void _Ready()
        {
            _selectButton = this.GetChild<Button>(recurse: false);
            _counterLabel = this.GetChild<Label>(recurse: false);

            if (_selectButton == null)
            {
                throw new InvalidOperationException($"{nameof(_selectButton)} is not set.");
            }

            if (_counterLabel == null)
            {
                throw new InvalidOperationException($"{nameof(_counterLabel)} is not set.");
            }

            if (!IsInstanceValid(CellComponent))
            {
                this.LogError($"{nameof(CellComponent)} is not set or the reference is invalid.");
            }

            _selectButton.Pressed += OnSelectButtonPressed;
        }

        private void OnSelectButtonPressed()
        {
            ComponentSelected?.Invoke(CellComponent);
        }
    }
}
