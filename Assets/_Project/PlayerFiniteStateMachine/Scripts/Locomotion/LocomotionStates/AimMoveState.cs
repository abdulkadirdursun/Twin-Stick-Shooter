using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public class AimMoveState : AbstractLocomotionState
    {
        #region Constructor

        public AimMoveState(LocomotionStateMachine stateMachine, LocomotionBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion

        public override void StateEnter()
        {
            base.StateEnter();
            Blackboard.GameplayInputs.StopAiming += ChangeToFreeMovementState;
            Blackboard.IKRigController.SetActive(true);
        }

        public override void StateUpdate()
        {
            var lookDirection = (Blackboard.AimTarget.position - Blackboard.MovementController.Position).normalized;
            lookDirection.y = 0f;
            Blackboard.LookDirection = lookDirection;
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();
            Blackboard.IKRigController.SetActive(false);
            Blackboard.GameplayInputs.StopAiming -= ChangeToFreeMovementState;
        }

        private void ChangeToFreeMovementState()
        {
            StateMachine.ChangeState<FreeMoveState>();
        }
    }
}