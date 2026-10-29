namespace EHE.CharacterSystem
{
    using Godot;

    public partial class Character : CharacterBody3D
    {
        private bool _isActive = false;

        private CharacterBehaviour _currentBehaviour = new DefaultMovementBehaviour();

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (_isActive)
            {
                _currentBehaviour.Execute();
            }
        }

        public void Activate()
        {
            _currentBehaviour.Initialize(this);
            _isActive = true;
        }
    }
}
