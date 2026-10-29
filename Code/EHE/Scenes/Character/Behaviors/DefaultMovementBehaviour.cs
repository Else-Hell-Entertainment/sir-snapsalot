using EHE.Global.Logging;
using Godot;
using SirSnapsalot.EHE.Global.Managers;

namespace EHE.CharacterSystem
{
    public partial class DefaultMovementBehaviour : CharacterBehaviour
    {
        public override BehaviorTypeDef BehaviorType { get; } = BehaviorTypeDef.Standard;

        public Vector3 TargetPosition { get; set; }

        public Vector3[] Path { get; set; }

        private int _currentPathIndex = 0;

        private bool _isMoving = false;

        private float _speed = 5f;

        private CharacterBody3D _body;

        public override void Initialize(Character character)
        {
            //TODO: Behaviour = node3D -> add to scene tree

            _body = character;
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
            Path = LevelManager.Instance.CurrentLevel.Grid.GetNavPath();
            if (Path.Length > 0)
            {
                TargetPosition = Path[0];
                _isMoving = true;
            }
        }

        private void Move()
        {
            float delta = (float)_body.GetPhysicsProcessDeltaTime();
            Vector3 direction = (TargetPosition - _body.GlobalPosition).Normalized();
            _body.GlobalPosition += direction * _speed * delta;

            if (_body.GlobalPosition.DistanceTo(TargetPosition) < 0.1f)
            {
                _body.GlobalPosition = TargetPosition;
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
