using System;
using TwinStickShooter.AnimationSystem.Enums;
using TwinStickShooter.WeaponSystem;
using TwinStickShooter.WeaponSystem.SlotSystem;
using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class AnimatorLayerController : IDisposable
    {
        #region Constructor

        public AnimatorLayerController(Animator animator, SelectedPlayerWeaponData selectedPlayerWeaponData)
        {
            _animator = animator;
            _selectedPlayerWeaponData = selectedPlayerWeaponData;
            _baseballBatLayerId = _animator.GetLayerIndex(AnimatorLayer.UpperBody_BaseballBat.ToString());
            _pistolLayerId = _animator.GetLayerIndex(AnimatorLayer.UpperBody_Pistol.ToString());
            _rifleLayerId = _animator.GetLayerIndex(AnimatorLayer.UpperBody_Rifle.ToString());

            _selectedPlayerWeaponData.SelectedWeaponChanged += SetAnimationLayerWeight;
            SetAnimationLayerWeight();
        }

        #endregion

        private readonly int _baseballBatLayerId;
        private readonly int _pistolLayerId;
        private readonly int _rifleLayerId;

        private readonly Animator _animator;
        private readonly SelectedPlayerWeaponData _selectedPlayerWeaponData;
        private int _activeLayerId = -1;

        public int ActiveLayerId => _activeLayerId;

        public AnimatorLayer ActiveAnimatorLayer => _selectedPlayerWeaponData.HasWeapon ? _selectedPlayerWeaponData.SelectedWeaponData.AnimatorLayer : AnimatorLayer.None;

        public void Dispose()
        {
            _selectedPlayerWeaponData.SelectedWeaponChanged -= SetAnimationLayerWeight;
        }

        private void SetAnimationLayerWeight()
        {
            ResetAnimationLayerWeight();
            if (!_selectedPlayerWeaponData.HasWeapon) return;

            _activeLayerId = GetLayerId(_selectedPlayerWeaponData.SelectedWeaponData.AnimatorLayer);
            if (_activeLayerId == -1) return;
            _animator.SetLayerWeight(_activeLayerId, 1f);
        }

        private void ResetAnimationLayerWeight()
        {
            if (_activeLayerId == -1) return;

            _animator.SetLayerWeight(_activeLayerId, 0f);
            _activeLayerId = -1;
        }

        private int GetLayerId(AnimatorLayer animatorLayer)
        {
            return animatorLayer switch
            {
                AnimatorLayer.UpperBody_BaseballBat => _baseballBatLayerId,
                AnimatorLayer.UpperBody_Pistol => _pistolLayerId,
                AnimatorLayer.UpperBody_Rifle => _rifleLayerId,
                _ => -1
            };
        }
    }
}