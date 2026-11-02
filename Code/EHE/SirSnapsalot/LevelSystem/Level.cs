// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.CharacterSystem;
using EHE.SirSnapsalot.LevelObjects;
using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class Level : Node3D
    {
        //TODO: Refactor temp solutions below:
        [Export]
        private FloorComponent _floorComponent;

        [Export]
        private WallComponent _wallComponent;

        [Export]
        private PropComponent _propComponent;

        [Export]
        private Button _startGameButton;

        [Export]
        private TrapComponent _pressurePlateComponent;

        [Export]
        private TrapComponent _pusherComponent;

        [Export]
        private Character _character;

        public LevelGrid Grid;

        private Vector3 _startPos = new Vector3(0, 0, 0);

        public override void _Ready()
        {
            Name = "Level";
            var grid = FindChild("LevelGrid");
            if (grid != null && grid is LevelGrid)
            {
                GD.Print("LevelGrid found in scene tree.");
                Grid = (LevelGrid)grid;
            }
            else if (Grid == null)
            {
                GD.Print("LevelGrid is null, creating a new instance.");
                Grid = new LevelGrid();
                Grid.Name = "LevelGrid";
                AddChild(Grid);
                var sceneOwner = GetTree().CurrentScene;
                if (sceneOwner != null)
                {
                    Grid.Owner = sceneOwner;
                }
            }
            //TODO: Refactor temp solutions below:

            _startGameButton.Pressed += StartGame;
            _startPos = _character.GlobalPosition;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            //TODO: Refactor temp solutions below:

            if (@event.IsActionPressed("Hotbar1"))
            {
                SetGhostComponent(_floorComponent);
                GD.Print("Selected Floor Component");
            }
            else if (@event.IsActionPressed("Hotbar2"))
            {
                SetGhostComponent(_wallComponent);
                GD.Print("Selected Wall Component");
            }
            else if (@event.IsActionPressed("Hotbar3"))
            {
                SetGhostComponent(_propComponent);
                GD.Print("Selected Prop Component");
            }
            else if (@event.IsActionPressed("RotateCW"))
            {
                Grid.RotateGhostComponent(true);
            }
            else if (@event.IsActionPressed("RotateCCW"))
            {
                Grid.RotateGhostComponent(false);
            }
            else if (@event.IsActionPressed("ClearCell"))
            {
                Grid.RemoveCell();
            }
            else if (@event.IsActionPressed("LevelEditorSelectPressurePlate"))
            {
                SetGhostComponent(_pressurePlateComponent);
            }
            else if (@event.IsActionPressed("LevelEditorSelectPusher"))
            {
                SetGhostComponent(_pusherComponent);
            }
        }

        /// <summary>
        /// Sets the current ghost component to the given cell component.
        /// </summary>
        /// <param name="ghostComponent">The reference to the cell component resource.</param>
        public void SetGhostComponent(CellComponent ghostComponent)
        {
            Grid.GhostComponent = ghostComponent;
            this.LogDebug($"Ghost component set to '{ghostComponent}'");
        }

        private void StartGame()
        {
            _character.GlobalPosition = _startPos;
            Grid.CalculateNavigation();

            //TODO: Refactor placeholder solution below:
            _character.ReceivePath(Grid.GetNavPath());
            _character.Activate();
            ShowPath();
        }

        private void ShowPath()
        {
            Grid.DrawNavigationPath();
        }
    }
}
