using System;
using TwinStickShooter.WeaponSystem;

namespace TwinStickShooter.WeaponSlotSystem
{
    [Serializable]
    public class WeaponSlot
    {
        public BaseWeaponData WeaponData { get; private set; }

        public event Action OnSlotChanged;

        public void EquipWeapon(BaseWeaponData weaponData)
        {
            WeaponData = weaponData;
            OnSlotChanged?.Invoke();
        }
    }
}