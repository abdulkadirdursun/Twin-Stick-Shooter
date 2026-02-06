using AKD.Toolkit.Singleton;
using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSlotSystem
{
    public class PlayerWeaponSlots : Singleton<PlayerWeaponSlots>
    {
        [SerializeField] private Transform weaponParent;
        private WeaponSlot[] _weaponSlots;

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
        
        #region MonoBehaviour Methods

        private void Awake()
        {
            _weaponSlots = new WeaponSlot[]
            {
                new WeaponSlot(weaponParent),
                new WeaponSlot(weaponParent),
                new WeaponSlot(weaponParent)
            };
        }

        #endregion
    }
}