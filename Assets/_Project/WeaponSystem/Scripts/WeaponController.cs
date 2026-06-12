using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private SelectedPlayerWeaponData selectedPlayerWeaponData;
        [SerializeField] private Transform weaponHandler;
        [SerializeField] private LayerMask weaponTargetLayers;

        private BaseWeapon _weapon;

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
            //TODO: Use Pool
            Destroy(_weapon.gameObject);
        }

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            selectedPlayerWeaponData.SelectedWeaponChanged += OnSelectedWeaponChanged;
        }

        private void OnDisable()
        {
            selectedPlayerWeaponData.SelectedWeaponChanged -= OnSelectedWeaponChanged;
        }

        #endregion
    }
}