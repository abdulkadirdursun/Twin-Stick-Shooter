using System;
using System.Collections.Generic;
using TwinStickShooter.StateMachineSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public sealed class LocomotionStateMachine : StateMachine<AbstractLocomotionState>
    {
        public LocomotionStateMachine(LocomotionBlackboard blackboard)
        {
            AvailableStates = new Dictionary<Type, AbstractLocomotionState>
            {
                { typeof(FreeMoveState), new FreeMoveState(this, blackboard) },
                { typeof(AimMoveState), new AimMoveState(this, blackboard) }
            };

            ChangeState<FreeMoveState>();
        }
    }
}