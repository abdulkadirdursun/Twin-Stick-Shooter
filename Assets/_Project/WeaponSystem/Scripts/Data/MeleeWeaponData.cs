using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    [CreateAssetMenu(fileName = "NewMeleeWeaponData", menuName = "Twin Stick Shooter/Weapon System/New Melee Weapon Data")]
    public class MeleeWeaponData : BaseWeaponData
    {
        [SerializeField, Range(0f, 1f)] private float hitWindowCloseTime = 1f;

        public float HitWindowCloseTime => hitWindowCloseTime;
    }
}