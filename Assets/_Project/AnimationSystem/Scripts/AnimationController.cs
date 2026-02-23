using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private AnimatorLayerController _animatorLayerController;

        public int GetActiveLayerId()
        {
            return _animatorLayerController.ActiveLayerId;
        }

        public void SetBool(int id, bool value)
        {
            animator.SetBool(id, value);
        }

        public void SetFloat(int id, float value)
        {
            animator.SetFloat(id, value);
        }

        public void SetTrigger(int id)
        {
            animator.SetTrigger(id);
        }

        #region MonoBehaviour Methods

        private void Start()
        {
            _animatorLayerController = new AnimatorLayerController(animator);
        }

        private void OnDestroy()
        {
            _animatorLayerController?.Dispose();
        }

        #endregion
    }
}