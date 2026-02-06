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

        public void EquipWeapon(BaseWeaponData weaponData, bool isActive)
        {
            WeaponData = weaponData;
            if (WeaponData)
            {
                SpawnWeapon(isActive);
            }
            else if (Weapon)
            {
                DestroyWeapon();
            }

            OnSlotChanged?.Invoke();
        }

        private void SpawnWeapon(bool isActive)
        {
            //TODO: Use Pool
            Weapon = Object.Instantiate(WeaponData.WeaponPrefab, _weaponParent, true);
            var rotationDifference = Quaternion.FromToRotation(Weapon.HoldTransform.forward, _weaponParent.forward);
            Weapon.transform.localRotation *= rotationDifference;
            Weapon.transform.localPosition = Weapon.HoldTransform.localPosition;
            Weapon.gameObject.SetActive(isActive);
        }

        private void DestroyWeapon()
        {
            Object.Destroy(Weapon.gameObject);
        }
    }
}