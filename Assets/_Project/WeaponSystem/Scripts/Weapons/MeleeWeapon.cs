using System.Collections.Generic;
using TwinStickShooter.Core;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class MeleeWeapon : BaseWeapon
    {
        [SerializeField] private MeleeWeaponData weaponData;
        [SerializeField] private MeleeHitDetector hitDetector;
        [SerializeField] private float hitCheckInterval = 0.2f;

        private readonly HashSet<IDamageable> _damagedTargets = new();
        protected override float AttackRate => weaponData.AttackRate;
        protected override bool RapidAttack => false;

        private bool _hitDetectionActive;
        private float _timeSinceLastCheck = 0f;

        public void EnableHitDetection()
        {
            _damagedTargets.Clear();
            _timeSinceLastCheck = 0f;
            _hitDetectionActive = true;
        }

        public void DisableHitDetection()
        {
            _hitDetectionActive = false;
        }

        public override void OnEquipped()
        {
            hitDetector.SetTargetLayers(TargetLayers);
        }

        public override void OnUnequipped()
        {
            base.OnUnequipped();
            _hitDetectionActive = false;
        }

        protected override void OnUpdate()
        {
            if (!_hitDetectionActive) return;

            _timeSinceLastCheck += Time.deltaTime;
            if (_timeSinceLastCheck < hitCheckInterval)
                return;
            _timeSinceLastCheck = 0f;

            if (!hitDetector.CheckCollision(out var damageTargets)) return;

            foreach (var target in damageTargets)
            {
                if (!_damagedTargets.Add(target)) continue;
                target.Damage(weaponData.Damage);
            }
        }
    }
}