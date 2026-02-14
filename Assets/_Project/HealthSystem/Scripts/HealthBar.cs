using PrimeTween;
using UnityEngine;

namespace AKD.HealthSystem
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Renderer healthBarRenderer;
        [SerializeField] private float pulseTime = 0.1f;

        private static readonly int FillId = Shader.PropertyToID("_Fill");
        private static readonly int EnablePulseId = Shader.PropertyToID("_EnablePulse");
        private static readonly int PulseTimeId = Shader.PropertyToID("_PulseTime");

        private MaterialPropertyBlock _materialPropertyBlock;
        private Tween _pulseTween;

        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            var fillAmount = currentHealth / maxHealth;
            UpdateHealthBar(fillAmount);
        }

        private void UpdateHealthBar(float healthPercent)
        {
            _materialPropertyBlock.SetFloat(FillId, healthPercent);
            healthBarRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void Pulse()
        {
            if (_pulseTween.isAlive) return;

            _materialPropertyBlock.SetFloat(EnablePulseId, 1f);
            healthBarRenderer.SetPropertyBlock(_materialPropertyBlock);
            _pulseTween = Tween.Delay(pulseTime, StopPulse);

            void StopPulse()
            {
                _materialPropertyBlock.SetFloat(EnablePulseId, 0f);
                healthBarRenderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            healthBarRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat(PulseTimeId, pulseTime);
            healthBarRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void OnEnable()
        {
            health.OnHealthChanged += UpdateHealthBar;
            health.OnDamaged += Pulse;
        }

        private void Start()
        {
            UpdateHealthBar(health.Percentage);
        }

        private void OnDisable()
        {
            health.OnHealthChanged -= UpdateHealthBar;
            health.OnDamaged -= Pulse;
        }

        #endregion
    }
}