using UnityEngine;

namespace TwinStickShooter.MovementSystem
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private Rigidbody movementRb;
        [SerializeField] private Transform rotationTarget;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float smoothTime = 0.08f;
        [SerializeField] private float rotationSpeed = 360f;

        private Vector3 _currentVelocity;
        private Vector3 _smoothedVelocity;

        public Vector3 Position => transform.position;

        public void Move(Vector3 moveDirection)
        {
            var targetVelocity = moveDirection * moveSpeed;
            _smoothedVelocity = Vector3.SmoothDamp(_smoothedVelocity, targetVelocity, ref _currentVelocity, smoothTime);
            var targetPosition = movementRb.position + _smoothedVelocity * Time.deltaTime;
            movementRb.MovePosition(targetPosition);
        }

        public void Rotate(Vector3 lookDirection)
        {
            if (lookDirection == Vector3.zero) return;
            var targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            rotationTarget.rotation = Quaternion.RotateTowards(rotationTarget.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public Vector3 LocalMoveDirection(Vector3 worldMoveDirection)
        {
            return rotationTarget.InverseTransformDirection(worldMoveDirection);
        }
    }
}