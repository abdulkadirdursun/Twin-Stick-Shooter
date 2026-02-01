using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class Gun : BaseWeapon
    {
        [SerializeField] private GunData gunData;
        protected override float AttackRate => gunData.AttackRate;

        protected override bool TryToAttack()
        {
            return false;
        }
    }
}