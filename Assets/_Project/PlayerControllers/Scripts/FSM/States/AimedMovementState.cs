using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerControllers.FSM
{
    public class AimedMovementState : BaseMovementState
    {
        #region Constructor

        public AimedMovementState(PlayerMovementStateMachine stateMachine, PlayerMovementBlackboard playerMovementBlackboard) : base(stateMachine,
            playerMovementBlackboard)
        {
            _mainCamera = Camera.main;
        }

        #endregion

        private Vector3 _lookDirection;
        private Camera _mainCamera;

        private const float GroundHeight = 0f;
        protected override Vector3 LookDirection => _lookDirection;

        protected override void OnStateEnter()
        {
            PlayerInputs.AimPositionInput += ReadAimRotationInput;
            PlayerInputs.OnStopAiming += ChangeToFreeMovementState;
        }

        protected override void OnStateExit()
        {
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
            _lookDirection = (mouseWorldPosition - Blackboard.Transform.position).normalized;
        }

        private void ChangeToFreeMovementState()
        {
            StateMachine.ChangeState<FreeMovementState>();
        }
    }
}