using TwinStickShooter.InputSystem;
using TwinStickShooter.StateMachineSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public abstract class AbstractLocomotionState: IState
    {
        #region Constructor

        protected AbstractLocomotionState(LocomotionStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        #endregion
        
        protected readonly LocomotionStateMachine StateMachine;
        protected abstract Vector3 LookDirection { get; }
        protected Vector3 MoveDirection;
        
        public virtual void StateEnter()
        {
            Debug.Log($"<color=green>[{GetType().Name}] </color>State Enter");
            PlayerInputs.MoveInput += ReadMoveInputInput;
        }

        public virtual void StateUpdate()
        {
            StateMachine.MovementController.Move(MoveDirection);
            StateMachine.MovementController.Rotate(LookDirection);
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