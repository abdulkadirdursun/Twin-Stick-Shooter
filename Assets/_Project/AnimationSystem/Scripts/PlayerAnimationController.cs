using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private AnimatorLayerController _animatorLayerController;

        #region Movement

        private bool _isMoving;
        private Vector3 _moveDirection;

        private static readonly int IsMovingId = Animator.StringToHash("IsMoving");
        private static readonly int MoveSpeedId = Animator.StringToHash("MoveSpeed");
        private static readonly int MoveDirectionXId = Animator.StringToHash("MoveDirectionX");
        private static readonly int MoveDirectionYId = Animator.StringToHash("MoveDirectionY");

        private void ReadMoveInputInput(Vector2 moveInput)
        {
            SetMoveState(moveInput != Vector2.zero);
            _moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        }

        private void SetMoveState(bool isMoving)
        {
            if (_isMoving == isMoving) return;
            _isMoving = isMoving;
            animator.SetBool(IsMovingId, _isMoving);
        }

        #endregion

        #region Combat

        private static readonly int AimId = Animator.StringToHash("Aim");
        private static readonly int ReloadId = Animator.StringToHash("Reload");
        private static readonly int AttackId = Animator.StringToHash("Attack");

        private void PlayAimAnimation()
        {
            animator.SetBool(AimId, true);
        }

        private void StopAimAnimation()
        {
            animator.SetBool(AimId, false);
        }

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            _animatorLayerController = new AnimatorLayerController(animator);
        }

        private void OnEnable()
        {
            PlayerInputs.MoveInput += ReadMoveInputInput;
            PlayerInputs.OnStartAiming += PlayAimAnimation;
            PlayerInputs.OnStopAiming += StopAimAnimation;
        }

        private void Update()
        {
            if (_isMoving)
            {
                var localMoveDirection = transform.InverseTransformDirection(_moveDirection);
                animator.SetFloat(MoveSpeedId, _moveDirection.magnitude);
                animator.SetFloat(MoveDirectionXId, localMoveDirection.x);
                animator.SetFloat(MoveDirectionYId, localMoveDirection.z);
            }
        }

        private void OnDisable()
        {
            PlayerInputs.MoveInput -= ReadMoveInputInput;
            PlayerInputs.OnStartAiming -= PlayAimAnimation;
            PlayerInputs.OnStopAiming -= StopAimAnimation;
        }

        private void OnDestroy()
        {
            _animatorLayerController?.Dispose();
        }

        #endregion
    }
}