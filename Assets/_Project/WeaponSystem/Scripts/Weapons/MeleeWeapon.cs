using System.Collections.Generic;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class MeleeWeapon : BaseWeapon
    {
        [SerializeField] private MeleeWeaponData weaponData;
        [SerializeField] private Collider hitCollider;

        //private HashSet<IDamageable> _damagedTargets = new();
        protected override float AttackRate => weaponData.AttackRate;
        protected override bool RapidAttack => false;

        public override void StartAttacking()
        {
            base.StartAttacking();
            hitCollider.enabled = true;
        }

        public override void StopAttacking()
        {
            //_damagedTargets.Clear();
            base.StopAttacking();
            hitCollider.enabled = false;
        }

        protected override void Attack()
        {
        }

        #region MonoBehaviour Methods

        private void OnTriggerEnter(Collider other)
        {
            //if (!other.TryGetComponent(out IDamageable target) || !_damagedTargets.Add(target)) return;
            //target.Damage(weaponData.Damage);
        }

        #endregion
    }
}