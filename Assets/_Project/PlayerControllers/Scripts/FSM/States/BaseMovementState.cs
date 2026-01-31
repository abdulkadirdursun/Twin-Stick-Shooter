using TwinStickShooter.InputSystem;
using TwinStickShooter.StateMachineSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerControllers.FSM
{
    public abstract class BaseMovementState : IState
    {
        #region Constructor

        protected BaseMovementState(PlayerMovementStateMachine stateMachine, PlayerMovementBlackboard blackboard)
        {
            StateMachine = stateMachine;
            Blackboard = blackboard;
        }

        #endregion

        protected PlayerMovementBlackboard Blackboard;
        protected PlayerMovementStateMachine StateMachine;
        protected Vector3 MoveDirection;
        protected abstract Vector3 LookDirection { get; }
        private float _rotationVelocity;
        private const float SmoothTime = 0.05f;

        public void StateEnter()
        {
            Debug.Log($"<color=green>[{GetType().Name}] </color>State Enter");
            PlayerInputs.MoveInput += ReadMoveInputInput;
            OnStateEnter();
        }

        public void StateUpdate()
        {
            Move();
            Rotate();
        }

        public void StateExit()
        {
            Debug.Log($"<color=magenta>[{GetType().Name}]</color> State Exit");
            PlayerInputs.MoveInput -= ReadMoveInputInput;
            OnStateExit();
        }

        protected virtual void OnStateEnter()
        {
        }

        protected virtual void OnStateExit()
        {
        }

        private void Move()
        {
            if (MoveDirection == Vector3.zero) return;
            var movement = MoveDirection * (Blackboard.MoveSpeed * Time.deltaTime);
            Blackboard.CharacterController.Move(movement);
        }

        private void Rotate()
        {
            if (LookDirection == Vector3.zero) return;
            var targetAngle = Mathf.Atan2(LookDirection.x, LookDirection.z) * Mathf.Rad2Deg;
            var smoothTargetRotation = Mathf.SmoothDampAngle(Blackboard.Transform.eulerAngles.y, targetAngle, ref _rotationVelocity, SmoothTime);
            var targetRotation = Quaternion.Euler(0f, smoothTargetRotation, 0f);
            Blackboard.Transform.rotation = targetRotation;
        }

        private void ReadMoveInputInput(Vector2 moveInput)
        {
            MoveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        }
    }
}