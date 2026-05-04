using System;
using UnityEditor;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class BaseHitDetector : MonoBehaviour
    {
        [Header("Shape")]
        [SerializeField] protected HitDetectionShape detectionShape;
        [SerializeField] protected Vector3 originOffset;
        [SerializeField] protected Vector3 size = Vector3.one;
        [SerializeField] protected float radius = 0.5f;
        [SerializeField] protected float height = 1f;
        [SerializeField] protected LocalAxis localUpAxis = LocalAxis.Y;
        [SerializeField] protected LocalAxis localForwardAxis = LocalAxis.Z;
        [Header("Gizmos")]
        [SerializeField] protected Color gizmosColor = Color.orangeRed;

        protected LayerMask TargetLayers;

        public void SetTargetLayers(LayerMask targetLayers)
        {
            TargetLayers = targetLayers;
        }

        protected Vector3 GetVector(LocalAxis localAxis)
        {
            return localAxis switch
            {
                LocalAxis.X => transform.right,
                LocalAxis.NX => -transform.right,
                LocalAxis.Y => transform.up,
                LocalAxis.NY => -transform.up,
                LocalAxis.Z => transform.forward,
                LocalAxis.NZ => -transform.forward,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #region Enums

        protected enum LocalAxis
        {
            X,
            NX,
            Y,
            NY,
            Z,
            NZ
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.orangeRed;
            switch (detectionShape)
            {
                case HitDetectionShape.Sphere:
                    Gizmos.DrawWireSphere(transform.position + originOffset, radius);
                    break;
                case HitDetectionShape.Capsule:
                    DrawCapsuleGizmos();
                    break;
                case HitDetectionShape.Box:
                    Gizmos.DrawWireCube(transform.position + originOffset, size);
                    break;
            }
        }

        private void DrawCapsuleGizmos()
        {
#if UNITY_EDITOR
            var upAxis = GetVector(localUpAxis);
            var forwardAxis = GetVector(localForwardAxis);
            var rightAxis = Vector3.Cross(upAxis, forwardAxis);

            var origin = transform.position + transform.TransformDirection(originOffset);
            float pointOffset = (height * 0.5f) - radius;
            var point1 = origin + upAxis * pointOffset;
            var point2 = origin - upAxis * pointOffset;

            Handles.color = gizmosColor;

            // Top hemisphere
            Handles.DrawWireDisc(point1, upAxis, radius);
            Handles.DrawWireArc(point1, rightAxis, forwardAxis, 180f, radius);
            Handles.DrawWireArc(point1, forwardAxis, rightAxis, -180f, radius);

            // Bottom hemisphere
            Handles.DrawWireDisc(point2, upAxis, radius);
            Handles.DrawWireArc(point2, rightAxis, forwardAxis, -180f, radius);
            Handles.DrawWireArc(point2, forwardAxis, rightAxis, 180f, radius);

            // Connecting lines
            Handles.DrawLine(point1 + forwardAxis * radius, point2 + forwardAxis * radius);
            Handles.DrawLine(point1 - forwardAxis * radius, point2 - forwardAxis * radius);
            Handles.DrawLine(point1 + rightAxis * radius, point2 + rightAxis * radius);
            Handles.DrawLine(point1 - rightAxis * radius, point2 - rightAxis * radius);
#endif
        }

        #endregion
    }
}