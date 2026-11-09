// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>
//         Miska Rihu <miska.rihu@tuni.fi>

using EHE.Common.Godot.Extensions;
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
        private Button _startGameButton;

        [Export]
        private Character _character;

        public LevelGrid Grid { get; private set; }

        private Entrance _entrance = null;
        private Exit _exit = null;

        /// <summary>
        /// The <see cref="LevelObjects.Entrance"/> object in this level.
        /// </summary>
        public Entrance Entrance
        {
            get
            {
                if (!IsInstanceValid(_entrance))
                {
                    _entrance = this.GetChild<Entrance>(recurse: true);
                }

                return _entrance;
            }
        }

        /// <summary>
        /// The <see cref="LevelObjects.Exit"/> object in this level.
        /// </summary>
        public Exit Exit
        {
            get
            {
                if (!IsInstanceValid(_exit))
                {
                    _exit = this.GetChild<Exit>(recurse: true);
                }

                return _exit;
            }
        }

        /// <summary>
        /// The GlobalPosition of the <see cref="Entrance"/> object.
        /// If no such object is present in the level, returns <see cref="Vector3.Inf"/>.
        /// </summary>
        public Vector3 EntrancePosition =>
            IsInstanceValid(Entrance)
                ? Entrance.GlobalPosition
                : Vector3.Inf;

        /// <summary>
        /// The GlobalPosition of the <see cref="Exit"/> object.
        /// If no such object is present in the level, returns <see cref="Vector3.Inf"/>.
        /// </summary>
        public Vector3 ExitPosition =>
            IsInstanceValid(Exit)
                ? Exit.GlobalPosition
                : Vector3.Inf;

        public override void _Ready()
        {
            Name = "Level";
            Grid = this.GetChild<LevelGrid>(recurse: true);

            if (Grid == null)
            {
                this.LogInfo($"{nameof(Grid)} is null, creating a new instance.");
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
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            //TODO: Refactor temp solutions below:

            if (@event.IsActionPressed("RotateCW"))
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
            ConnectActivatorsToTraps();

            _character.GlobalPosition = EntrancePosition;
            Grid.CalculateNavigation();

            //TODO: Refactor placeholder solution below:
            _character.ReceivePath(Grid.GetNavPath(_character.GlobalPosition, ExitPosition));
            _character.Activate();
            ShowPath();
        }

        private void ShowPath()
        {
            Grid.DrawNavigationPath(_character.GlobalPosition, ExitPosition);
        }

        private void ConnectActivatorsToTraps()
        {
            foreach (var activator in this.GetChildren<TrapActivator>(recurse: true))
            {
                // Find activators and clear their existing connections.
                activator.ClearConnections();

                foreach (var gridCell in Grid.GetNeighbors(activator.GlobalPosition))
                {
                    // Find neighboring cells and if they contain a trap, connect it to the activator.
                    var trap = gridCell.GetChild<Trap>(recurse: true);
                    if (trap != null)
                    {
                        activator.AddConnection(trap);
                    }
                }
            }
        }
    }
}
