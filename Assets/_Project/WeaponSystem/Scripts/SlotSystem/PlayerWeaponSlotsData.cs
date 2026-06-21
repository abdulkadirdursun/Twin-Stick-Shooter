using UnityEngine;

namespace TwinStickShooter.WeaponSystem.SlotSystem
{
    [CreateAssetMenu(fileName = "PlayerWeaponSlotsData", menuName = "Twin Stick Shooter/Weapon System/Slot System/Player Weapon Slots Data")]
    public class PlayerWeaponSlotsData : ScriptableObject
    {
        private WeaponSlot[] _weaponSlots;

        private const int SlotCount = 3;

        public void Initialize()
        {
            _weaponSlots = new WeaponSlot[]
            {
                new WeaponSlot(),
                new WeaponSlot(),
                new WeaponSlot()
            };
        }

        public bool TryGetWeaponSlotData(int index, out WeaponSlot weaponSlot)
        {
            weaponSlot = null;
            if (_weaponSlots == null)
            {
                Debug.LogError("Weapon slots are not initialized!!!");
                return false;
            }

            if (index >= SlotCount)
            {
                Debug.LogError($"Requested slot index ({index}) out of slot count bound!!");
                return false;
            }

            weaponSlot = _weaponSlots[index];
            return true;
        }

        public void AddWeaponToEmptySlot(BaseWeaponData weaponData)
        {
            if (weaponData == null || !TryToGetEmptySlot(out var emptySlot)) return;
            emptySlot.AddWeapon(weaponData);
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
    }
}