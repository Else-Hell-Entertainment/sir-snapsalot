using Godot;

namespace EHE.Global.FSM
{
    public abstract partial class NodeState : Node3D
    {
        public abstract void Enter();
        public abstract void Exit();
    }
}
