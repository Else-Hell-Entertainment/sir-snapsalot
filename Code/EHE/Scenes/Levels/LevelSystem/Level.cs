using EHE.CharacterSystem;
using Godot;

namespace EHE.LevelSystem
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
                Grid.GhostComponent = _floorComponent;
                GD.Print("Selected Floor Component");
            }
            else if (@event.IsActionPressed("Hotbar2"))
            {
                Grid.GhostComponent = _wallComponent;
                GD.Print("Selected Wall Component");
            }
            else if (@event.IsActionPressed("Hotbar3"))
            {
                Grid.GhostComponent = _propComponent;
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
