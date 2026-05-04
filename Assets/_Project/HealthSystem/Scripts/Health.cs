using System;
using TwinStickShooter.DamageableSystem;
using UnityEngine;

namespace AKD.HealthSystem
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100;
        private float _currentHealth;

        /// <summary>
        /// float 1: Current Health<br/>
        /// float 2: Max Health
        /// </summary>
        public event Action<float, float> OnHealthChanged;
        public event Action OnDamaged;
        public event Action OnDied;
        public bool IsAlive => _currentHealth > 0;
        public float Percentage => _currentHealth > 0 && maxHealth > 0f ? _currentHealth / maxHealth : 0f;

        private void Initialize()
        {
            _currentHealth = maxHealth;
        }

        public void Damage(float damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0f;
                OnDied?.Invoke();
            }

            OnDamaged?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            Initialize();
        }

        #endregion
    }
}