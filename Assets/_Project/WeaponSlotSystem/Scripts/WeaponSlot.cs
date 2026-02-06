using System;
using TwinStickShooter.WeaponSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TwinStickShooter.WeaponSlotSystem
{
    [Serializable]
    public class WeaponSlot
    {
        #region Constructor

        public WeaponSlot(Transform weaponParent)
        {
            _weaponParent = weaponParent;
        }

        #endregion

        public BaseWeaponData WeaponData { get; private set; }
        public BaseWeapon Weapon { get; private set; }

        private Transform _weaponParent;

        public event Action OnSlotChanged;

        public void EquipWeapon(BaseWeaponData weaponData)
        {
            WeaponData = weaponData;
            if (WeaponData)
            {
                SpawnWeapon();
            }
            else if (Weapon)
            {
                DestroyWeapon();
            }

            OnSlotChanged?.Invoke();
        }

        private void SpawnWeapon()
        {
            Weapon = Object.Instantiate(WeaponData.WeaponPrefab, _weaponParent);
        }

        private void DestroyWeapon()
        {
            Object.Destroy(Weapon.gameObject);
        }
    }
}