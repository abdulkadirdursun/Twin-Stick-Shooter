using TwinStickShooter.DamageableSystem;
using UnityEditor;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class ProjectileHitDetector : MonoBehaviour
    {
        [Header("Shape")]
        [SerializeField] private DetectionShape detectionShape;
        [SerializeField] private Vector3 originOffset;
        [SerializeField] private Vector3 size = Vector3.one;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float height = 1f;
        [Header("Gizmos")]
        [SerializeField] private Color gizmosColor = Color.orangeRed;

        private readonly RaycastHit[] _rayHitBuffer = new RaycastHit[1];
        private LayerMask _targetLayers;

        public void SetTargetLayers(LayerMask targetLayers)
        {
            _targetLayers = targetLayers;
        }

        public bool CheckCollision(Vector3 direction, float distance, out IDamageable damageTarget)
        {
            int hitCount = 0;
            var origin = transform.position + originOffset;
            switch (detectionShape)
            {
                case DetectionShape.Sphere:
                    hitCount = Physics.SphereCastNonAlloc(origin, radius, direction, _rayHitBuffer, distance, _targetLayers);
                    break;
                case DetectionShape.Capsule:
                    var upVector = transform.up;
                    var pointOffset = (height * 0.5f) - radius;
                    var point1 = origin + (upVector * pointOffset);
                    var point2 = origin - (upVector * pointOffset);
                    hitCount = Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, _rayHitBuffer, distance, _targetLayers);
                    break;
                case DetectionShape.Box:
                    hitCount = Physics.BoxCastNonAlloc(origin, size * 0.5f, direction, _rayHitBuffer, transform.rotation, distance, _targetLayers);
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

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.orangeRed;
            switch (detectionShape)
            {
                case DetectionShape.Sphere:
                    Gizmos.DrawWireSphere(transform.position + originOffset, radius);
                    break;
                case DetectionShape.Capsule:
                    DrawCapsuleGizmo();
                    break;
                case DetectionShape.Box:
                    Gizmos.DrawWireCube(transform.position + originOffset, size);
                    break;
            }
        }

        private void DrawCapsuleGizmo()
        {
#if UNITY_EDITOR
            Handles.color = gizmosColor;
            var origin = transform.position + originOffset;

            Matrix4x4 angleMatrix = Matrix4x4.TRS(origin, Quaternion.identity, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(angleMatrix))
            {
                float pointOffset = (height - radius * 2f) / 2f;

                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180f, radius);
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180f, radius);
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);

                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180f, radius);
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180f, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);

                Handles.DrawLine(new Vector3(radius, pointOffset, 0f), new Vector3(radius, -pointOffset, 0f));
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0f), new Vector3(-radius, -pointOffset, 0f));
                Handles.DrawLine(new Vector3(0f, pointOffset, radius), new Vector3(0f, -pointOffset, radius));
                Handles.DrawLine(new Vector3(0f, pointOffset, -radius), new Vector3(0f, -pointOffset, -radius));
            }
#endif
        }

        #endregion

        #region Enums

        private enum DetectionShape
        {
            Sphere,
            Capsule,
            Box
        }

        #endregion
    }
}