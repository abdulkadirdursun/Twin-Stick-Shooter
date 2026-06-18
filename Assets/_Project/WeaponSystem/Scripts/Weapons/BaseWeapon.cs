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
        protected LayerMask TargetLayers;
        public bool OnCooldown => _timeSinceLastShot > 0f;

        public void Equip(AnimationEventDispatcher animationEventDispatcher)
        {
            AnimationEventDispatcher = animationEventDispatcher;
            OnEquipped();
            SubscribeAnimationEvents();
        }

        public void Unequip()
        {
            UnsubscribeAnimationEvents();
            OnUnequipped();
        }

        public bool TryToAttack(out FailedAttackReason failedAttackReason)
        {
            if (!CanAttack(out failedAttackReason)) return false;
            _timeSinceLastShot = AttackRate;
            PerformAttack();
            return true;
        }

        protected virtual bool CanAttack(out FailedAttackReason failedAttackReason)
        {
            failedAttackReason = FailedAttackReason.None;
            if (OnCooldown)
            {
                failedAttackReason = FailedAttackReason.Cooldown;
                return false;
            }

            return true;
        }

        public void SetTargetLayers(LayerMask targetLayers)
        {
            TargetLayers = targetLayers;
        }

        public abstract void ShowDamageAreaPreview(bool isVisible);

        protected abstract void PerformAttack();
        protected abstract void OnEquipped();
        protected abstract void OnUnequipped();
        protected abstract void OnDropped();
        protected abstract void SubscribeAnimationEvents();
        protected abstract void UnsubscribeAnimationEvents();

        protected virtual void OnUpdate()
        {
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (OnCooldown)
            {
                _timeSinceLastShot -= Time.deltaTime;
            }

            OnUpdate();
        }

        #endregion
    }
}