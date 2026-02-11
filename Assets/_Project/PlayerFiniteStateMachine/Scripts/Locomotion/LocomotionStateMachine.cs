using System;
using System.Collections.Generic;
using TwinStickShooter.MovementSystem;
using TwinStickShooter.StateMachineSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public sealed class LocomotionStateMachine : StateMachine<AbstractLocomotionState>
    {
        #region Constructor

        public LocomotionStateMachine(MovementController movementController)
        {
            MovementController = movementController;
            InitializeStateMachine();
        }

        #endregion

        public MovementController MovementController { get; }

        public override void InitializeStateMachine()
        {
            AvailableStates = new Dictionary<Type, AbstractLocomotionState>
            {
                { typeof(FreeMoveState), new FreeMoveState(this) },
                { typeof(AimMoveState), new AimMoveState(this) }
            };

            ChangeState<FreeMoveState>();
        }
    }
}