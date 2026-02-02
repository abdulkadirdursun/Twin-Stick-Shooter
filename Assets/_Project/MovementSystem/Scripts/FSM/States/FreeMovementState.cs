using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.MovementSystem.FSM
{
    public class FreeMovementState : BaseMovementState
    {
        #region Constructor

        public FreeMovementState(PlayerMovementStateMachine stateMachine, PlayerMovementBlackboard playerMovementBlackboard) : base(stateMachine,
            playerMovementBlackboard)
        {
        }

        #endregion

        protected override Vector3 LookDirection => MoveDirection;

        protected override void OnStateEnter()
        {
            PlayerInputs.OnStartAiming += ChangeToAimingState;
        }

        protected override void OnStateExit()
        {
            PlayerInputs.OnStartAiming -= ChangeToAimingState;
        }

        private void ChangeToAimingState()
        {
            StateMachine.ChangeState<AimedMovementState>();
        }
    }
}