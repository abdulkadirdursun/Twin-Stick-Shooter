using System;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    [CreateAssetMenu(fileName = "SelectedPlayerWeaponData", menuName = "Twin Stick Shooter/Weapon System/Selected Player Weapon Data")]
    public class SelectedPlayerWeaponData : ScriptableObject
    {
        public BaseWeaponData SelectedWeaponData { get; private set; }
        public bool HasWeapon => SelectedWeaponData != null;

        public event Action SelectedWeaponChanged;

        public void SetSelectedWeaponData(BaseWeaponData weaponData)
        {
            SelectedWeaponData = weaponData;
            SelectedWeaponChanged?.Invoke();
        }
    }
}