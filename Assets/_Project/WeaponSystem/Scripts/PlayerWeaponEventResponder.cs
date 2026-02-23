using TwinStickShooter.WeaponSlotSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class PlayerWeaponEventResponder : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponSlots playerWeaponSlots;

        public void OnShootAnimationStart()
        {
            if (playerWeaponSlots.ActiveSlot?.Weapon is not Gun gun) return;

            gun.Shoot();
        }

        public void EnableHitDetection()
        {
            if (playerWeaponSlots.ActiveSlot?.Weapon is not MeleeWeapon meleeWeapon) return;
            
            meleeWeapon.EnableHitDetection();
        }

        public void DisableHitDetection()
        {
            if (playerWeaponSlots.ActiveSlot?.Weapon is not MeleeWeapon meleeWeapon) return;
            
            meleeWeapon.DisableHitDetection();
        }
    }
}