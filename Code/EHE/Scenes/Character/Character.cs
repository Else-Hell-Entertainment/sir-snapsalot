namespace EHE.CharacterSystem
{
    using Godot;

    public partial class Character : Node3D
    {
        [Export]
        public float Speed { get; set; } = 5.0f;

        public Vector3 TargetPosition { get; set; }

        public Vector3[] Path { get; set; }

        private int _currentPathIndex = 0;

        private bool _isMoving = false;

        private CharacterBehavior _currentBehavior = new DefaultMovementBehavior();

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (_isMoving)
            {
                Move(delta);
            }
        }

        public void ReceivePath(Vector3[] path)
        {
            Path = path;
            if (Path.Length > 0)
            {
                TargetPosition = Path[0];
            }
        }

        public void Activate()
        {
            if (Path != null && Path.Length > 0)
            {
                _isMoving = true;
                TargetPosition = Path[0];
            }
        }

        private void Move(double delta)
        {
            if (_isMoving)
            {
                Vector3 direction = (TargetPosition - GlobalPosition).Normalized();
                GlobalPosition += direction * Speed * (float)delta;

                if (GlobalPosition.DistanceTo(TargetPosition) < 0.1f)
                {
                    GlobalPosition = TargetPosition;
                    _isMoving = false;
                    _currentPathIndex++;
                    ResolveNextPoint();
                }
            }
        }

        private void ResolveNextPoint()
        {
            if (_currentPathIndex < Path.Length)
            {
                TargetPosition = Path[_currentPathIndex];
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
                _currentPathIndex = 0;
                GD.Print("Character has reached the end of the path.");
            }
        }
    }
}
