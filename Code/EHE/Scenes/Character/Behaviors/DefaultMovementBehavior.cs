using Godot;

namespace EHE.CharacterSystem
{
    public partial class DefaultMovementBehavior : CharacterBehavior
    {
        public override BehaviorTypeDef BehaviorType { get; } = BehaviorTypeDef.Standard;

        public Vector3 TargetPosition { get; set; }

        public Vector3[] Path { get; set; }

        private int _currentPathIndex = 0;

        private bool _isMoving = false;

        private float Speed = 5f;

        public override void Initialize(Character character)
        {
            UpdatePath();
        }

        public override void Execute()
        {
            if (_isMoving)
            {
                Move();
            }
        }

        private void UpdatePath()
        {
            //Path = LevelManager.Instance.CurrentLevel.Grid.GetNavPath();
            if (Path.Length > 0)
            {
                TargetPosition = Path[0];
            }
        }

        private void Move()
        {
            float delta = (float)GetPhysicsProcessDeltaTime();
            Vector3 direction = (TargetPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * Speed * delta;

            if (GlobalPosition.DistanceTo(TargetPosition) < 0.1f)
            {
                GlobalPosition = TargetPosition;
                _isMoving = false;
                _currentPathIndex++;
                ResolveNextPoint();
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
