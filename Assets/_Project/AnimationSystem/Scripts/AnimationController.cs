using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private AnimatorLayerController _animatorLayerController;

        public void SetBool(int id, bool value)
        {
            animator.SetBool(id,value);
        }

        public void SetFloat(int id, float value)
        {
            animator.SetFloat(id,value);
        }

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
            PlayerInputs.OnStartAiming += PlayAimAnimation;
            PlayerInputs.OnStopAiming += StopAimAnimation;
        }

        private void OnDisable()
        {
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