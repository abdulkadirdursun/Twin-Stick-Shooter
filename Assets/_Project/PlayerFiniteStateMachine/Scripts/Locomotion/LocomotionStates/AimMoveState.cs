using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public class AimMoveState : AbstractLocomotionState
    {
        #region Constructor

        public AimMoveState(LocomotionStateMachine stateMachine) : base(stateMachine)
        {
            _mainCamera = Camera.main;
        }

        #endregion

        private readonly Camera _mainCamera;
        private Vector3 _lookDirection;
        private const float GroundHeight = 0f;
        protected override Vector3 LookDirection => _lookDirection;

        public override void StateEnter()
        {
            base.StateEnter();
            PlayerInputs.AimPositionInput += ReadAimRotationInput;
            PlayerInputs.OnStopAiming += ChangeToFreeMovementState;
        }

        public override void StateExit()
        {
            base.StateExit();
            PlayerInputs.AimPositionInput -= ReadAimRotationInput;
            PlayerInputs.OnStopAiming -= ChangeToFreeMovementState;
        }

        private void ReadAimRotationInput(Vector2 input)
        {
            if (InputControlScheme.CurrentControlType == ControlSchemeType.Gamepad)
            {
                _lookDirection = new Vector3(input.x, 0f, input.y);
                return;
            }

            var distanceToGround = Mathf.Abs(_mainCamera.transform.position.y - GroundHeight);
            var screenPositionWithDepth = new Vector3(input.x, input.y, distanceToGround);
            var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(screenPositionWithDepth);
            _lookDirection = (mouseWorldPosition - StateMachine.MovementController.Position).normalized;
        }

        private void ChangeToFreeMovementState()
        {
            StateMachine.ChangeState<FreeMoveState>();
        }
    }
}