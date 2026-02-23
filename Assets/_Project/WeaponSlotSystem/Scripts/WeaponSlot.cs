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
        public bool IsActive { get; private set; }

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

        public void SetActive(bool isActive)
        {
            if(IsActive==isActive)return;
            IsActive = isActive;
            Weapon?.gameObject.SetActive(IsActive);

        }

        private void SpawnWeapon()
        {
            //TODO: Use Pool
            Weapon = Object.Instantiate(WeaponData.WeaponPrefab, _weaponParent, true);
            var rotationDifference = Quaternion.FromToRotation(Weapon.HoldTransform.forward, _weaponParent.forward);
            Weapon.transform.localRotation *= rotationDifference;
            Weapon.transform.localPosition = Weapon.HoldTransform.localPosition;
            Weapon.OnEquipped();
            Weapon.gameObject.SetActive(IsActive);
        }

        private void DestroyWeapon()
        {
            Object.Destroy(Weapon.gameObject);
        }
    }
}