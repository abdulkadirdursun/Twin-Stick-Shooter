using System;
using TwinStickShooter.AnimationSystem.Enums;
using TwinStickShooter.WeaponSlotSystem;
using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    //TODO: Refactor
    public class AnimatorLayerController : IDisposable
    {
        #region Constructor

        public AnimatorLayerController(Animator animator)
        {
            _animator = animator;
            _baseballBatLayerId = _animator.GetLayerIndex(AnimatorLayer.UpperBody_BaseballBat.ToString());
            _pistolLayerId = _animator.GetLayerIndex(AnimatorLayer.UpperBody_Pistol.ToString());
            _rifleLayerId = _animator.GetLayerIndex(AnimatorLayer.UpperBody_Rifle.ToString());

            if (!PlayerWeaponSlots.IsInstanceExist)
            {
                Debug.LogError($"{nameof(PlayerWeaponSlots)} instance is not available");
                return;
            }

            PlayerWeaponSlots.Instance.OnActiveSlotChanged += OnWeaponSlotChanged;
            OnWeaponSlotChanged(PlayerWeaponSlots.Instance.ActiveSlot);
        }

        #endregion

        private readonly int _baseballBatLayerId;
        private readonly int _pistolLayerId;
        private readonly int _rifleLayerId;

        private readonly Animator _animator;
        private WeaponSlot _weaponSlot;
        private int _activeLayerId = -1;

        public void Dispose()
        {
            if (!PlayerWeaponSlots.IsInstanceExist) return;
            PlayerWeaponSlots.Instance.OnActiveSlotChanged -= OnWeaponSlotChanged;
        }

        private void OnWeaponSlotChanged(WeaponSlot weaponSlot)
        {
            if (_activeLayerId != -1)
            {
                _animator.SetLayerWeight(_activeLayerId, 0f);
            }

            if (_weaponSlot != null)
            {
                _weaponSlot.OnSlotChanged -= SetLayerWeight;
            }

            _weaponSlot = weaponSlot;

            if (_weaponSlot != null)
            {
                _weaponSlot.OnSlotChanged += SetLayerWeight;
            }

            if (_weaponSlot == null)
            {
                _activeLayerId = -1;
                return;
            }

            SetLayerWeight();
        }

        private void SetLayerWeight()
        {
            if (_activeLayerId != -1)
            {
                _animator.SetLayerWeight(_activeLayerId, 0f);
            }

            if (!_weaponSlot.Weapon)
            {
                _activeLayerId = -1;
                return;
            }

            _activeLayerId = GetLayerId(_weaponSlot.WeaponData.AnimatorLayer);
            _animator.SetLayerWeight(_activeLayerId, 1f);
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