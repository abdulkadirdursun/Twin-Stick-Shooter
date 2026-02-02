using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class MeleeWeapon : BaseWeapon
    {
        [SerializeField] protected MeleeWeaponData weaponData;
        protected override float AttackRate => weaponData.AttackRate;

        protected override bool TryToAttack()
        {
            return false;
        }
    }
}