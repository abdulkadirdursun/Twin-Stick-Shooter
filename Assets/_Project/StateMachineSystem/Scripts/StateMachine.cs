using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStickShooter.StateMachineSystem
{
    public abstract class StateMachine<T> : IDisposable where T : IState
    {
        private T _currentState;
        private T _previousState;

        protected Dictionary<Type, T> AvailableStates;

        public void ChangeState<TState>() where TState : T
        {
            if (!AvailableStates.TryGetValue(typeof(TState), out T state))
            {
                Debug.LogError($"State of {typeof(TState).Name} is not available for {GetType().Name}");
                return;
            }

            ChangeState(state);
        }

        public void CallStateUpdate()
        {
            if (_currentState == null) return;
            _currentState.StateUpdate();
        }

        /// <typeparam name="TState">Default State Type in case of previous state is null</typeparam>
        public void ReturnToPreviousState<TState>() where TState : T
        {
            if (_previousState == null)
            {
                ChangeState<TState>();
                return;
            }

            if (_previousState.Equals(_currentState)) return;

            ChangeState(_previousState);
        }

        private void ChangeState(T newState)
        {
            if (_currentState != null)
                _currentState.StateExit();
            _previousState = _currentState;
            _currentState = newState;
            _currentState.StateEnter();
        }

        public void Dispose()
        {
            if (_currentState == null) return;
            _currentState.StateExit();
        }
    }
}