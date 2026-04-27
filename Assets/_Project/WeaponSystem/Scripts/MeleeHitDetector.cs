using System;
using System.Collections.Generic;
using TwinStickShooter.Core;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class MeleeHitDetector : BaseHitDetector
    {
        [SerializeField] private int bufferSize;

        private Collider[] _hitColliderBuffer;

        public bool CheckCollision(out List<IDamageable> damageTargets)
        {
            int hitCount = 0;
            damageTargets = new List<IDamageable>();
            var origin = transform.position + originOffset;
            switch (detectionShape)
            {
                case HitDetectionShape.Sphere:
                    hitCount = Physics.OverlapSphereNonAlloc(origin, radius, _hitColliderBuffer, TargetLayers);
                    break;
                case HitDetectionShape.Capsule:
                    var upVector = GetVector(localUpAxis);
                    var pointOffset = (height * 0.5f) - radius;
                    var point0 = origin + (upVector * pointOffset);
                    var point1 = origin - (upVector * pointOffset);
                    hitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, _hitColliderBuffer, TargetLayers);
                    break;
                case HitDetectionShape.Box:
                    hitCount = Physics.OverlapBoxNonAlloc(origin, size * 0.5f, _hitColliderBuffer, transform.rotation, TargetLayers);
                    break;
            }

            if (hitCount == 0)
                return false;

            for (int i = 0; i < hitCount; i++)
            {
                var hitInfo = _hitColliderBuffer[i];
                if (!hitInfo.TryGetComponent(out IDamageable damageTarget)) continue;
                damageTargets.Add(damageTarget);
            }

            return damageTargets.Count > 0;
        }

        

        #region MonoBehaviour Methods

        private void Awake()
        {
            _hitColliderBuffer = new Collider[bufferSize];
        }

        #endregion

        
    }
}