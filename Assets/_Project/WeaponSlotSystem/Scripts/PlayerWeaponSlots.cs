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
        private WeaponSlot[] _weaponSlots;
        public WeaponSlot ActiveSlot { get; private set; }
        public event Action<WeaponSlot> OnActiveSlotChanged;

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
            ActiveSlot?.SetActive(false);
            ActiveSlot = selectedSlot;
            ActiveSlot?.SetActive(true);
            OnActiveSlotChanged?.Invoke(ActiveSlot);
        }

        #region MonoBehaviour Methods

        protected override void OnAwake()
        {
            _weaponSlots = new WeaponSlot[]
            {
                new WeaponSlot(weaponParent),
                new WeaponSlot(weaponParent),
                new WeaponSlot(weaponParent)
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