using System;

namespace TwinStickShooter.WeaponSystem.SlotSystem
{
    [Serializable]
    public class WeaponSlot
    {
        public BaseWeaponData WeaponData { get; private set; }

        public event Action OnSlotChanged;

        public void AddWeapon(BaseWeaponData weaponData)
        {
            WeaponData = weaponData;
            OnSlotChanged?.Invoke();
        }
    }
}