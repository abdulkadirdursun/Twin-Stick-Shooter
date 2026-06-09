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

        private static readonly int MoveSpeedId = Animator.StringToHash("MoveSpeed");
        private static readonly int MoveDirectionXId = Animator.StringToHash("MoveDirectionX");
        private static readonly int MoveDirectionYId = Animator.StringToHash("MoveDirectionY");

        public virtual void StateEnter()
        {
            Debug.Log($"<color=green>[{GetType().Name}] </color>State Enter");
            Blackboard.GameplayInputs.MoveInput += ReadMoveInputInput;
        }

        public virtual void StateUpdate()
        {
            Blackboard.MovementController.Move(Blackboard.MoveDirection);
            var lookRotation = Blackboard.LookDirection;
            Blackboard.MovementController.Rotate(lookRotation);

            if (Blackboard.MoveDirection != Vector3.zero)
            {
                var localDirection = Blackboard.MovementController.LocalMoveDirection(Blackboard.MoveDirection);
                Blackboard.AnimationController.SetFloat(MoveDirectionXId, localDirection.x);
                Blackboard.AnimationController.SetFloat(MoveDirectionYId, localDirection.z);
            }

            Blackboard.AnimationController.SetFloat(MoveSpeedId, Blackboard.MoveDirection.magnitude);
        }

        public virtual void StateExit()
        {
            Debug.Log($"<color=magenta>[{GetType().Name}]</color> State Exit");
            Blackboard.GameplayInputs.MoveInput -= ReadMoveInputInput;
        }

        private void ReadMoveInputInput(Vector2 moveInput)
        {
            Blackboard.MoveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        }
    }
}