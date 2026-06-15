using System;
using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private SelectedPlayerWeaponData selectedPlayerWeaponData;
        [SerializeField] private GameplayInputs gameplayInputs;
        [SerializeField] private Transform weaponHandler;
        [SerializeField] private LayerMask weaponTargetLayers;

        private BaseWeapon _weapon;
        private bool _canAttack;

        public event Action AttackTriggered;

        public void ChangeAttackPermit(bool value)
        {
            if (!_weapon)
            {
                _canAttack = false;
                return;
            }

            if (value == _canAttack)
            {
                Debug.LogWarning($"Attack permit already set to {value}");
                return;
            }

            _canAttack = value;
            _weapon.ShowDamageAreaPreview(_canAttack);
        }

        public void Reload()
        {
            Debug.LogWarning("[WeaponController] Reload");
            if (_weapon == null || _weapon is not Gun gun) return;
            gun.Reload();
        }

        public void DisableMeleeWeaponHitDetection() //Temp
        {
            if (!_weapon || _weapon is not MeleeWeapon meleeWeapon) return;
            meleeWeapon.DisableHitDetection();
        }

        private void Attack()
        {
            if (!_weapon || !_canAttack) return;
            if (!_weapon.TryToAttack(out var failedAttackReason)) return;
            AttackTriggered?.Invoke();
        }

        private void OnSelectedWeaponChanged()
        {
            HideCurrentWeapon();
            if (!selectedPlayerWeaponData.HasWeapon) return;
            var weaponData = selectedPlayerWeaponData.SelectedWeaponData;
            _weapon = Instantiate(weaponData.WeaponPrefab, weaponHandler, true);
            var weaponTransform = _weapon.transform;
            weaponTransform.localRotation = Quaternion.Euler(weaponData.HoldRotation);
            weaponTransform.localPosition = weaponData.HoldPosition;
            _weapon.SetTargetLayers(weaponTargetLayers);
            _weapon.OnEquipped();
        }

        private void HideCurrentWeapon()
        {
            if (_weapon == null) return;
            _weapon.OnUnequipped();
            //TODO: Use Pool
            Destroy(_weapon.gameObject);
        }

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            selectedPlayerWeaponData.SelectedWeaponChanged += OnSelectedWeaponChanged;
            gameplayInputs.AttackPressed += Attack;
        }

        private void OnDisable()
        {
            selectedPlayerWeaponData.SelectedWeaponChanged -= OnSelectedWeaponChanged;
            gameplayInputs.AttackPressed -= Attack;
        }

        #endregion
    }
}