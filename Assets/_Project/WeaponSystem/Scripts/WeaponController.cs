using System;
using AKD.AnimationEvents;
using TwinStickShooter.AnimationSystem;
using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private SelectedPlayerWeaponData selectedPlayerWeaponData;
        [SerializeField] private GameplayInputs gameplayInputs;
        [SerializeField] private AnimationController animationController;
        [SerializeField] private AnimationEventDispatcher animationEventDispatcher;
        [SerializeField] private Transform weaponHandler;
        [SerializeField] private LayerMask weaponTargetLayers;

        private readonly int _attackParameterId = Animator.StringToHash("Attack");
        private BaseWeapon _weapon;
        private bool _canAttack;

        public event Action<BaseWeapon, BaseWeaponData> WeaponEquipped;
        public event Action WeaponUnequipped;

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

        private void StartAttack()
        {
            if (!_weapon || !_canAttack) return;
            _weapon.AttackPerformed += CallAttackAnimation;
            _weapon.StartAttack();
        }

        private void StopAttack()
        {
            if (!_weapon) return;
            _weapon.StopAttack();
            _weapon.AttackPerformed -= CallAttackAnimation;
        }

        private void CallAttackAnimation()
        {
            animationController.SetTrigger(_attackParameterId);
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
            _weapon.Equip(animationEventDispatcher);
            WeaponEquipped?.Invoke(_weapon, weaponData);
        }

        private void HideCurrentWeapon()
        {
            if (_weapon == null) return;
            _weapon.Unequip();
            WeaponUnequipped?.Invoke();
            //TODO: Use Pool
            Destroy(_weapon.gameObject);
        }

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            selectedPlayerWeaponData.SelectedWeaponChanged += OnSelectedWeaponChanged;
            gameplayInputs.AttackPressed += StartAttack;
            gameplayInputs.AttackReleased += StopAttack;
        }

        private void Update()
        {
            if (!_weapon) return;
            _weapon.Tick(Time.deltaTime);
        }

        private void OnDisable()
        {
            selectedPlayerWeaponData.SelectedWeaponChanged -= OnSelectedWeaponChanged;
            gameplayInputs.AttackPressed -= StartAttack;
            gameplayInputs.AttackReleased -= StopAttack;
        }

        #endregion
    }
}