using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class WeaponAimLineDrawer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform firePoint;

        private bool _isActive;
        private float _maxDistance = 100f;
        private readonly RaycastHit[] _rayHitBuffer = new RaycastHit[1];
        private readonly Vector3[] _linePositionsBuffer = new Vector3[2];

        public void Configure(float maxDistance)
        {
            _maxDistance = maxDistance;
        }

        public void SetActive(bool value)
        {
            _isActive = value;
            lineRenderer.enabled = _isActive;
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (!_isActive) return;
            var hitCount = Physics.RaycastNonAlloc(firePoint.position, firePoint.forward, _rayHitBuffer, _maxDistance);
            _linePositionsBuffer[0] = firePoint.position;
            _linePositionsBuffer[1] = hitCount > 0 ? _rayHitBuffer[0].point : firePoint.forward * _maxDistance;
            lineRenderer.SetPositions(_linePositionsBuffer);
        }

        #endregion
    }
}