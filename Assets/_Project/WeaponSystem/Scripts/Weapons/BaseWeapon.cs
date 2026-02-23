using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        [SerializeField] private Transform holdTransform;
        protected abstract float AttackRate { get; }
        protected abstract bool RapidAttack { get; }
        private float _timeSinceLastShot;
        protected bool IsAttacking;

        public Transform HoldTransform => holdTransform;
        public bool OnCooldown => _timeSinceLastShot < AttackRate;

        public virtual void AttackPressed()
        {
            if (IsAttacking) return;
            IsAttacking = true;
        }

        public virtual void AttackReleased()
        {
            IsAttacking = false;
        }
        
        public virtual void OnStartAim(){}
        public virtual void OnStopAim(){}

        public virtual bool CanAttack(out FailedAttackReason failedAttackReason)
        {
            if (OnCooldown)
            {
                failedAttackReason = FailedAttackReason.Cooldown;
                return false;
            }

            if (!IsAttacking)
            {
                failedAttackReason = FailedAttackReason.None;
                return false;
            }
            
            if (!RapidAttack)
            {
                IsAttacking = false;
            }

            _timeSinceLastShot = 0f;
            failedAttackReason = FailedAttackReason.None;
            return true;
        }


        public virtual void OnEquipped()
        {
        }

        public virtual void OnUnequipped()
        {
            IsAttacking = false;
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (_timeSinceLastShot < AttackRate)
            {
                _timeSinceLastShot += Time.deltaTime;
            }
        }

        #endregion
    }
}