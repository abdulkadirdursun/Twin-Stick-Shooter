using System.Collections.Generic;
using AKD.AnimationEvents;
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

        private readonly int _attackAnimationId = Animator.StringToHash("Attack");

        protected override void PerformAttack()
        {
            _damagedTargets.Clear();
            _timeSinceLastCheck = 0f;
            _hitDetectionActive = true;
        }

        public override void ShowDamageAreaPreview(bool isVisible)
        {
        }

        protected override void OnEquipped()
        {
            hitDetector.SetTargetLayers(TargetLayers);
        }

        protected override void OnUnequipped()
        {
            _hitDetectionActive = false;
        }

        protected override void OnDropped()
        {
            _hitDetectionActive = false;
        }

        protected override void SubscribeAnimationEvents()
        {
            AnimationEventDispatcher.Register(AnimationEventScope.State(_attackAnimationId), weaponData.HitWindowCloseTime, DisableHitDetection);
        }

        protected override void UnsubscribeAnimationEvents()
        {
        }

        protected override void OnTicked(float time)
        {
            if (!_hitDetectionActive) return;

            _timeSinceLastCheck += time;
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

        private void DisableHitDetection(AnimationEventContext context)
        {
            _hitDetectionActive = false;
        }
    }
}