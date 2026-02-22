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
        public bool CanAttack => _timeSinceLastShot >= AttackRate;

        public virtual void AttackPressed()
        {
            if (IsAttacking) return;
            IsAttacking = true;
        }

        public virtual void AttackReleased()
        {
            IsAttacking = false;
        }

        public virtual void OnAttackAnimationStartEventTriggered()
        {
        }

        public virtual void OnAttackAnimationEndEventTriggered()
        {
        }

        public virtual bool TryToAttack(out FailedAttackReason failedAttackReason)
        {
            if (!CanAttack)
            {
                failedAttackReason = FailedAttackReason.Cooldown;
                return false;
            }

            if (!IsAttacking)
            {
                failedAttackReason = FailedAttackReason.None;
                return false;
            }

            Attack();
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

        protected abstract void Attack();

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