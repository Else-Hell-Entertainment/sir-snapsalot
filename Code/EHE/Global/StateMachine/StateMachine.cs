using System;
using System.Collections.Generic;
using EHE.Global.Logging;
using Godot;

namespace EHE.Global.FSM
{
    public class StateMachine
    {
        public IState CurrentState => _stateHistory.Count > 0 ? _stateHistory.Peek() : null;

        public event Action<IState> StateChanged;

        private readonly Stack<IState> _stateHistory = new();

        public bool ChangeState(IState newState)
        {
            if (newState == null)
            {
                this.LogError("Attempted to execute a transition to non-existing state.");
                return false;
            }

            if (CurrentState == newState)
            {
                this.LogDebug($"Already in state {newState}, no transition needed.");
                return true;
            }

            // Handle case where the new state already exists within the stack.
            if (_stateHistory.Contains(newState))
            {
                this.LogDebug($"State {newState} already exists in the state history, rewinding to it.");

                RewindStateHistory(newState);
                newState.Enter();
                StateChanged?.Invoke(newState);
                return true;
            }

            // Target state was not loaded already.
            // Call soft exit on the current state if it exists, but keep loaded in stack.
            if (newState.IsAdditive)
            {
                this.LogDebug(
                    $"State {newState} is additive, calling soft exit on current state and pushing new state onto the stack."
                );

                CurrentState?.Exit(true);
                _stateHistory.Push(newState);
                newState.Enter();
                StateChanged?.Invoke(newState);
                return true;
            }

            // Target state is not additive, which means the state history needs to be cleared entirely first.
            this.LogDebug(
                $"State {newState} is not additive, clearing state history and pushing new state onto the stack."
            );

            while (_stateHistory.Count > 0)
            {
                _stateHistory.Pop().Exit();
            }

            _stateHistory.Push(newState);
            newState.Enter();
            StateChanged?.Invoke(newState);
            return true;
        }

        public bool TransitionToPrevious()
        {
            if (_stateHistory.Count < 2)
            {
                GD.PrintErr($"Failed to transition to previous state, no previous state available.");
                return false;
            }

            IState currentState = _stateHistory.Pop();
            IState previousState = _stateHistory.Peek();

            currentState.Exit();
            previousState.Enter();
            StateChanged?.Invoke(previousState);
            return true;
        }

        public void Update()
        {
            CurrentState?.Execute();
        }

        private void RewindStateHistory(IState targetState)
        {
            GD.Print("Rewinding state history:");
            while (_stateHistory.Count > 0)
            {
                var state = _stateHistory.Peek();
                GD.Print($"Found state {state}");
                if (state == targetState)
                {
                    GD.Print("Rewinding state history finished.");
                    break;
                }

                _stateHistory.Pop().Exit();
            }
        }
    }
}
