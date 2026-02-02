using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSlotSystem
{
    [CreateAssetMenu(fileName = "PlayerWeaponSlotsData", menuName = "Twin Stick Shooter/Weapon Control System/Player Weapon Slots Data")]
    public class PlayerWeaponSlotsData : ScriptableObject
    {
        public WeaponSlot[] WeaponSlots { get; } = new WeaponSlot[]
        {
            new WeaponSlot(),
            new WeaponSlot(),
            new WeaponSlot()
        };

        public bool TryToEquip(BaseWeaponData weaponData)
        {
            if (HasTheWeapon(weaponData) || !TryToGetEmptySlot(out var emptySlot))
                return false;

            emptySlot.EquipWeapon(weaponData);
            return true;
        }

        private bool TryToGetEmptySlot(out WeaponSlot emptySlot)
        {
            foreach (var weaponSlot in WeaponSlots)
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
            foreach (var weaponSlot in WeaponSlots)
            {
                if (weaponSlot.WeaponData && weaponSlot.WeaponData == weaponData)
                    return true;
            }

            return false;
        }
    }
}