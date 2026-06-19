using System;
using AKD.AnimationEvents;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        [SerializeField] private Transform holdTransform;
        protected abstract float AttackRate { get; }
        protected abstract bool RapidAttack { get; }
        protected AnimationEventDispatcher AnimationEventDispatcher { get; private set; }
        private float _timeSinceLastShot;
        private bool _canAttack;
        protected LayerMask TargetLayers;
        public bool OnCooldown => _timeSinceLastShot > 0f;
        public event Action AttackPerformed;

        public void Equip(AnimationEventDispatcher animationEventDispatcher)
        {
            AnimationEventDispatcher = animationEventDispatcher;
            OnEquipped();
            SubscribeAnimationEvents();
        }

        public void Unequip()
        {
            AttackPerformed = null;
            UnsubscribeAnimationEvents();
            OnUnequipped();
        }

        public void StartAttack()
        {
            Debug.LogWarning("Start Attack");
            _canAttack = true;
        }

        public void StopAttack()
        {
            _canAttack = false;
        }

        public void Tick(float time)
        {
            _timeSinceLastShot -= time;
            if (!OnCooldown)
                TryToAttack();

            OnTicked(time);
        }

        private void TryToAttack()
        {
            if (!CanAttack()) return;
            _timeSinceLastShot = AttackRate;
            PerformAttack();
            AttackPerformed?.Invoke();
            Debug.Log("Attack Performed");
            if (!RapidAttack)
            {
                Debug.Log("Not rapid fire");
                _canAttack = false;
            }
        }

        protected virtual bool CanAttack()
        {
            if (!_canAttack || OnCooldown)
                return false;

            return true;
        }

        public void SetTargetLayers(LayerMask targetLayers)
        {
            TargetLayers = targetLayers;
        }

        protected virtual void OnTicked(float time)
        {
        }

        public abstract void ShowDamageAreaPreview(bool isVisible);
        protected abstract void PerformAttack();
        protected abstract void OnEquipped();
        protected abstract void OnUnequipped();
        protected abstract void OnDropped();
        protected abstract void SubscribeAnimationEvents();
        protected abstract void UnsubscribeAnimationEvents();
    }
}