using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        [SerializeField] private Transform holdTransform;
        protected abstract float AttackRate { get; }
        protected abstract bool RapidAttack { get; }
        private float _timeSinceLastShot;
        protected LayerMask TargetLayers;
        public bool OnCooldown => _timeSinceLastShot < AttackRate;

        public bool TryToAttack(out FailedAttackReason failedAttackReason)
        {
            if (!CanAttack(out failedAttackReason)) return false;
            _timeSinceLastShot = 0f;
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
        
        public abstract void OnEquipped();

        public abstract void OnUnequipped();

        public abstract void OnDropped();

        protected virtual void OnUpdate()
        {
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (_timeSinceLastShot < AttackRate)
            {
                _timeSinceLastShot += Time.deltaTime;
            }

            OnUpdate();
        }

        #endregion
    }
}