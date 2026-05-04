using System;
using AKD.Toolkit.Singleton;
using TwinStickShooter.InputSystem;
using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSlotSystem
{
    public class PlayerWeaponSlots : Singleton<PlayerWeaponSlots>
    {
        [SerializeField] private Transform weaponParent;
        [SerializeField] private LayerMask targetLayers;
        private WeaponSlot[] _weaponSlots;
        public WeaponSlot ActiveSlot { get; private set; }
        public event Action OnActiveSlotChanged;

        public bool TryToEquip(BaseWeaponData weaponData)
        {
            if (HasTheWeapon(weaponData) || !TryToGetEmptySlot(out var emptySlot))
                return false;

            emptySlot.EquipWeapon(weaponData);
            return true;
        }

        public WeaponSlot GetWeaponSlot(int index)
        {
            return index >= _weaponSlots.Length ? null : _weaponSlots[index];
        }

        public bool ActiveSlotHasWeapon(out BaseWeapon weapon)
        {
            weapon = null;
            if (ActiveSlot == null) return false;
            weapon = ActiveSlot.Weapon;
            return weapon != null;
        }

        private bool TryToGetEmptySlot(out WeaponSlot emptySlot)
        {
            foreach (var weaponSlot in _weaponSlots)
            {
                if (weaponSlot.WeaponData) continue;
                emptySlot = weaponSlot;
                return true;
            }

            emptySlot = null;
            return false;
        }

        private bool HasTheWeapon(BaseWeaponData weaponData)
        {
            foreach (var weaponSlot in _weaponSlots)
            {
                if (weaponSlot.WeaponData && weaponSlot.WeaponData == weaponData)
                    return true;
            }

            return false;
        }

        private void OnSlotSelected(int slotNumber)
        {
            var index = slotNumber - 1;
            SelectActiveSlot(index);
        }

        private void SelectActiveSlot(int slotIndex)
        {
            var selectedSlot = _weaponSlots[slotIndex];
            if (selectedSlot.IsActive) return;
            if (ActiveSlot != null)
            {
                ActiveSlot.Weapon?.OnUnequipped();
                ActiveSlot.SetActive(false);
            }

            ActiveSlot = selectedSlot;
            ActiveSlot?.SetActive(true);
            OnActiveSlotChanged?.Invoke();
        }

        #region MonoBehaviour Methods

        protected override void OnAwake()
        {
            _weaponSlots = new WeaponSlot[]
            {
                new WeaponSlot(weaponParent, targetLayers),
                new WeaponSlot(weaponParent, targetLayers),
                new WeaponSlot(weaponParent, targetLayers)
            };

            SelectActiveSlot(0);
        }

        private void OnEnable()
        {
            PlayerInputs.OnWeaponSlotSelected += OnSlotSelected;
        }

        private void OnDisable()
        {
            PlayerInputs.OnWeaponSlotSelected -= OnSlotSelected;
        }

        #endregion
    }
}