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

        public virtual void StartAttacking()
        {
            IsAttacking = true;
        }

        public virtual void StopAttacking()
        {
            IsAttacking = false;
        }

        public virtual bool TryToAttack()
        {
            if (!IsAttacking || !CanAttack) return false;

            Attack();
            if (!RapidAttack)
            {
                StopAttacking();
            }

            _timeSinceLastShot = 0f;
            return true;
        }


        public virtual void OnEquipped()
        {
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