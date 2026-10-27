// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using Godot;

namespace EHE.SirSnapsalot.FSM
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
