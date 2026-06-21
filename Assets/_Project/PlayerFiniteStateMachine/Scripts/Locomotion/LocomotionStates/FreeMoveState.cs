using TwinStickShooter.InputSystem;
using TwinStickShooter.MovementSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public class FreeMoveState : AbstractLocomotionState
    {
        #region Constructor

        public FreeMoveState(LocomotionStateMachine stateMachine, LocomotionBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion

        public override void StateEnter()
        {
            base.StateEnter();
            Blackboard.GameplayInputs.StartAiming += ChangeToAimingState;
        }

        public override void StateUpdate()
        {
            Blackboard.LookDirection = Blackboard.MoveDirection;
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();
            Blackboard.GameplayInputs.StartAiming -= ChangeToAimingState;
        }

        private void ChangeToAimingState()
        {
            StateMachine.ChangeState<AimMoveState>();
        }
    }
}