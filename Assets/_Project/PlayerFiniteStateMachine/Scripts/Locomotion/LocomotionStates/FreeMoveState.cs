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

        protected override Vector3 LookDirection => MoveDirection;

        public override void StateEnter()
        {
            base.StateEnter();
            PlayerInputs.OnStartAiming += ChangeToAimingState;
        }

        public override void StateExit()
        {
            base.StateExit();
            PlayerInputs.OnStartAiming -= ChangeToAimingState;
        }
        
        private void ChangeToAimingState()
        {
            StateMachine.ChangeState<AimMoveState>();
        }
    }
}