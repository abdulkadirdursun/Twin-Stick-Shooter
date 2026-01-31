using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerControllers
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        #region Movement

        private bool _isMoving;

        private static readonly int IsMovingId = Animator.StringToHash("IsMoving");
        private static readonly int MoveSpeedId = Animator.StringToHash("MoveSpeed");
        private static readonly int MoveDirectionXId = Animator.StringToHash("MoveDirectionX");
        private static readonly int MoveDirectionYId = Animator.StringToHash("MoveDirectionY");

        private void ReadMoveInputInput(Vector2 moveInput)
        {
            SetMoveState(moveInput != Vector2.zero);
            animator.SetFloat(MoveSpeedId, moveInput.magnitude);
            animator.SetFloat(MoveDirectionXId, moveInput.x);
            animator.SetFloat(MoveDirectionYId, moveInput.y);
        }

        private void SetMoveState(bool isMoving)
        {
            if (_isMoving == isMoving) return;
            _isMoving = isMoving;
            animator.SetBool(IsMovingId, _isMoving);
        }

        #endregion

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            PlayerInputs.MoveInput += ReadMoveInputInput;
        }

        private void OnDisable()
        {
            PlayerInputs.MoveInput -= ReadMoveInputInput;
        }

        #endregion
    }
}