using TwinStickShooter.WeaponSlotSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class PlayerWeaponEventResponder : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponSlots playerWeaponSlots;

        public void OnWeaponAttackAnimationStartEvent()
        {
            if (!playerWeaponSlots.ActiveSlot?.Weapon) return;
            playerWeaponSlots.ActiveSlot.Weapon.OnAttackAnimationStartEventTriggered();
        }

        public void OnWeaponAttackAnimationEndEvent()
        {
            if (!playerWeaponSlots.ActiveSlot?.Weapon) return;
            playerWeaponSlots.ActiveSlot.Weapon.OnAttackAnimationEndEventTriggered();
        }
    }
}