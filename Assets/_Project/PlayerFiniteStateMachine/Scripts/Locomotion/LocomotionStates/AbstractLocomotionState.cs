using TwinStickShooter.InputSystem;
using TwinStickShooter.StateMachineSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public abstract class AbstractLocomotionState : IState
    {
        #region Constructor

        protected AbstractLocomotionState(LocomotionStateMachine stateMachine, LocomotionBlackboard blackboard)
        {
            StateMachine = stateMachine;
            Blackboard = blackboard;
        }

        #endregion

        protected readonly LocomotionStateMachine StateMachine;
        protected readonly LocomotionBlackboard Blackboard;
        protected abstract Vector3 LookDirection { get; }
        protected Vector3 MoveDirection;

        private static readonly int MoveSpeedId = Animator.StringToHash("MoveSpeed");
        private static readonly int MoveDirectionXId = Animator.StringToHash("MoveDirectionX");
        private static readonly int MoveDirectionYId = Animator.StringToHash("MoveDirectionY");

        public virtual void StateEnter()
        {
            Debug.Log($"<color=green>[{GetType().Name}] </color>State Enter");
            PlayerInputs.MoveInput += ReadMoveInputInput;
        }

        public virtual void StateUpdate()
        {
            Blackboard.MovementController.Move(MoveDirection);
            Blackboard.MovementController.Rotate(LookDirection);

            if (MoveDirection != Vector3.zero)
            {
                var localDirection = Blackboard.MovementController.LocalMoveDirection(MoveDirection);
                Blackboard.AnimationController.SetFloat(MoveSpeedId,MoveDirection.magnitude);
                Blackboard.AnimationController.SetFloat(MoveDirectionXId,localDirection.x);
                Blackboard.AnimationController.SetFloat(MoveDirectionYId,localDirection.z);
            }
        }

        public virtual void StateExit()
        {
            Debug.Log($"<color=magenta>[{GetType().Name}]</color> State Exit");
            PlayerInputs.MoveInput -= ReadMoveInputInput;
        }

        private void ReadMoveInputInput(Vector2 moveInput)
        {
            MoveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        }
    }
}