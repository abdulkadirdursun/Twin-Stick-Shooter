using System.Collections.Generic;
using TwinStickShooter.DamageableSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class MeleeWeapon : BaseWeapon
    {
        [SerializeField] private MeleeWeaponData weaponData;
        [SerializeField] private Collider hitCollider;

        private HashSet<IDamageable> _damagedTargets = new();
        protected override float AttackRate => weaponData.AttackRate;
        protected override bool RapidAttack => false;

        public override void StartAttacking()
        {
            base.StartAttacking();
            _damagedTargets.Clear();
            hitCollider.enabled = true;
        }

        protected override void Attack()
        {
        }

        private void OnAttackComplete()
        {
            hitCollider.enabled = false;
        }

        #region MonoBehaviour Methods

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target) || !_damagedTargets.Add(target)) return;
            target.Damage(weaponData.Damage);
        }

        #endregion
    }
}