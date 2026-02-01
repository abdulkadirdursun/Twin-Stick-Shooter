using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        protected abstract float AttackRate { get; }
        private float _timeSinceLastShot;

        protected abstract bool TryToAttack();

        #region MonoBehaviour Methods

        private void Update()
        {
            if (_timeSinceLastShot >= AttackRate)
            {
                if (TryToAttack())
                    _timeSinceLastShot = 0;
                return;
            }

            _timeSinceLastShot += Time.deltaTime;
        }

        #endregion
    }
}