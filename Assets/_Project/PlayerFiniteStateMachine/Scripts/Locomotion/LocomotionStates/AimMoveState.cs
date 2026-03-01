using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public class AimMoveState : AbstractLocomotionState
    {
        #region Constructor

        public AimMoveState(LocomotionStateMachine stateMachine, LocomotionBlackboard blackboard) : base(stateMachine, blackboard)
        {
            _mainCamera = Camera.main;
        }

        #endregion

        private readonly Camera _mainCamera;
        private const float GroundHeight = 0f;

        public override void StateEnter()
        {
            base.StateEnter();
            PlayerInputs.AimPositionInput += ReadAimRotationInput;
            PlayerInputs.OnStopAiming += ChangeToFreeMovementState;
            Blackboard.IKAimTargetPlacer.SetActive(true);
        }

        public override void StateExit()
        {
            base.StateExit();
            Blackboard.IKAimTargetPlacer.SetActive(false);
            PlayerInputs.AimPositionInput -= ReadAimRotationInput;
            PlayerInputs.OnStopAiming -= ChangeToFreeMovementState;
        }

        private void ReadAimRotationInput(Vector2 input)
        {
            if (InputControlScheme.CurrentControlType == ControlSchemeType.Gamepad)
            {
                Blackboard.LookDirection = new Vector3(input.x, 0f, input.y);
            }
            else
            {
                var distanceToGround = Mathf.Abs(_mainCamera.transform.position.y - GroundHeight);
                var screenPositionWithDepth = new Vector3(input.x, input.y, distanceToGround);
                var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(screenPositionWithDepth);
                Blackboard.LookDirection = (mouseWorldPosition - Blackboard.MovementController.Position).normalized;
            }

            Blackboard.IKAimTargetPlacer.SetDirection(Blackboard.LookDirection);
        }

        private void ChangeToFreeMovementState()
        {
            StateMachine.ChangeState<FreeMoveState>();
        }
    }
}