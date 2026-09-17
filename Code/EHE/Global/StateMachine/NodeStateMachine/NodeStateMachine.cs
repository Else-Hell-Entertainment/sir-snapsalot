using System;
using Godot;

namespace EHE.Global.FSM
{
    public partial class NodeStateMachine : Node3D
    {
        private NodeState _defaultState;

        private NodeState _currentState;

        public void ChangeState(NodeState newState)
        {
            if (_currentState != null)
            {
                _currentState.Exit();
                RemoveChild(_currentState);
            }

            _currentState = newState;
            AddChild(_currentState);
            newState.Enter();
        }

        public void ExitCurrentState()
        {
            if (_currentState != null)
            {
                _currentState.Exit();
                RemoveChild(_currentState);
                _currentState = null;
            }

            if (_defaultState != null)
            {
                ChangeState(_defaultState);
            }
        }

        public void SetDefaultState(NodeState defaultState)
        {
            _defaultState = defaultState;
            ChangeState(defaultState);
        }
    }
}
