using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStickShooter.StateMachineSystem
{
    public abstract class StateMachine<T> where T : IState
    {
        private T _currentState;

        protected Dictionary<Type, T> AvailableStates;
        
        public void ChangeState<TState>() where TState : T
        {
            if (!AvailableStates.TryGetValue(typeof(TState), out T state))
            {
                Debug.LogError($"State of {typeof(TState).Name} is not available for {GetType().Name}");
                return;
            }

            if (_currentState != null)
                _currentState.StateExit();
            _currentState = state;
            _currentState.StateEnter();
        }

        public void CallStateUpdate()
        {
            if (_currentState == null) return;
            _currentState.StateUpdate();
        }
    }
}