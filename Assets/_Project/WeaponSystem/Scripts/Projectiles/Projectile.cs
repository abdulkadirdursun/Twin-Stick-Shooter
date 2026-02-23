using TwinStickShooter.DamageableSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed;

        private bool _isActive;
        private float _damage;
        private float _maxDistance;
        private float _distanceTravelled;

        public void Fire(float damage, float maxDistance)
        {
            _damage = damage;
            _maxDistance = maxDistance;
            _distanceTravelled = 0f;
            _isActive = true;
        }

        private void Disable()
        {
            if (!_isActive) return;
            _isActive = false;
            ProjectilePool.Instance.ReleaseProjectile(this);
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (!_isActive) return;
            var moveDistance = speed * Time.deltaTime;
            transform.position += (transform.forward * moveDistance);
            _distanceTravelled += moveDistance;
            if (_distanceTravelled < _maxDistance) return;
            Disable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive || !other.TryGetComponent(out IDamageable target)) return;
            target.Damage(_damage);
            Disable();
        }

        #endregion
    }
}