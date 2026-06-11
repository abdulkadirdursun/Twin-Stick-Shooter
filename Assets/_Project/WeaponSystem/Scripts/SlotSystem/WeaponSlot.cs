using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TwinStickShooter.WeaponSystem.SlotSystem
{
    [Serializable]
    public class WeaponSlot
    {
        #region Constructor

        public WeaponSlot(Transform weaponParent, LayerMask targetLayers)
        {
            _weaponParent = weaponParent;
            _targetLayers = targetLayers;
        }

        #endregion

        public BaseWeaponData WeaponData { get; private set; }
        public BaseWeapon Weapon { get; private set; }

        private Transform _weaponParent;
        private LayerMask _targetLayers;
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
            if (IsActive == isActive) return;
            IsActive = isActive;
            Weapon?.gameObject.SetActive(IsActive);
        }

        private void SpawnWeapon()
        {
            //TODO: Use Pool
            Weapon = Object.Instantiate(WeaponData.WeaponPrefab, _weaponParent, true);
            Weapon.transform.localRotation = Quaternion.Euler(WeaponData.HoldRotation);
            Weapon.transform.localPosition = WeaponData.HoldPosition;
            Weapon.OnEquipped(_targetLayers);
            Weapon.gameObject.SetActive(IsActive);
        }

        private void DestroyWeapon()
        {
            Object.Destroy(Weapon.gameObject);
        }
    }
}