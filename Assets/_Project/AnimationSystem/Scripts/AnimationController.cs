using System;
using TwinStickShooter.AnimationSystem.Enums;
using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private SelectedPlayerWeaponData selectedPlayerWeaponData;
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private Animator animator;

        private AnimatorLayerController _animatorLayerController;

        private readonly int _attackParameterId = Animator.StringToHash("Attack");

        public int GetActiveLayerId()
        {
            return _animatorLayerController.ActiveLayerId;
        }

        public AnimatorLayer GetActiveLayer()
        {
            return _animatorLayerController.ActiveAnimatorLayer;
        }

        private void OnAttackTriggered()
        {
            SetTrigger(_attackParameterId);
        }

        #region Animator Calls

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

        #endregion

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            weaponController.AttackTriggered += OnAttackTriggered;
        }

        private void Start()
        {
            _animatorLayerController = new AnimatorLayerController(animator, selectedPlayerWeaponData);
        }

        private void OnDisable()
        {
            weaponController.AttackTriggered -= OnAttackTriggered;
        }

        private void OnDestroy()
        {
            _animatorLayerController?.Dispose();
        }

        #endregion
    }
}