using TwinStickShooter.Core;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class ProjectileHitDetector : BaseHitDetector
    {
        private readonly RaycastHit[] _rayHitBuffer = new RaycastHit[1];

        public bool CheckCollision(Vector3 direction, float distance, out IDamageable damageTarget)
        {
            int hitCount = 0;
            var origin = transform.position + originOffset;
            switch (detectionShape)
            {
                case HitDetectionShape.Sphere:
                    hitCount = Physics.SphereCastNonAlloc(origin, radius, direction, _rayHitBuffer, distance, TargetLayers);
                    break;
                case HitDetectionShape.Capsule:
                    var upVector = transform.up;
                    var pointOffset = (height * 0.5f) - radius;
                    var point1 = origin + (upVector * pointOffset);
                    var point2 = origin - (upVector * pointOffset);
                    hitCount = Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, _rayHitBuffer, distance, TargetLayers);
                    break;
                case HitDetectionShape.Box:
                    hitCount = Physics.BoxCastNonAlloc(origin, size * 0.5f, direction, _rayHitBuffer, transform.rotation, distance, TargetLayers);
                    break;
            }

            if (hitCount == 0)
            {
                damageTarget = null;
                return false;
            }

            var hitInfo = _rayHitBuffer[0];
            if (!hitInfo.collider.TryGetComponent(out damageTarget)) return false;
            return true;
        }
    }
}