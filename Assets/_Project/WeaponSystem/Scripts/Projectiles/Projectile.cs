using System;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileHitDetector hitDetector;
        [SerializeField] private float speed;

        private bool _isActive;
        private float _damage;
        private float _maxDistance;
        private float _distanceTravelled;
        public event Action<Projectile> Destroyed;

        public void Fire(float damage, float maxDistance, LayerMask targetLayers)
        {
            _damage = damage;
            _maxDistance = maxDistance;
            _distanceTravelled = 0f;
            hitDetector.SetTargetLayers(targetLayers);
            _isActive = true;
        }

        private void Disable()
        {
            if (!_isActive) return;
            _isActive = false;
            Destroyed?.Invoke(this);
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (!_isActive) return;
            var moveDirection = transform.forward;
            var moveDistance = speed * Time.deltaTime;
            if (hitDetector.CheckCollision(moveDirection, moveDistance, out var damageTarget))
            {
                damageTarget.Damage(_damage);
                Disable();
                return;
            }

            transform.position += (transform.forward * moveDistance);
            _distanceTravelled += moveDistance;
            if (_distanceTravelled < _maxDistance) return;
            Disable();
        }

        private void OnDestroy()
        {
            Destroyed = null;
        }

        #endregion
    }
}