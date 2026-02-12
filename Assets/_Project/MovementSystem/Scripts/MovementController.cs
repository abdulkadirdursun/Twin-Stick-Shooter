using UnityEngine;

namespace TwinStickShooter.MovementSystem
{
    public class MovementController:MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;
        
        private float _rotationVelocity;
        private const float SmoothTime = 0.05f;

        public Vector3 Position => transform.position;
        
        public void Move(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero) return;
            var movement = moveDirection * (moveSpeed * Time.deltaTime);
            characterController.Move(movement);
        }

        public void Rotate(Vector3 lookDirection)
        {
            if (lookDirection == Vector3.zero) return;
            var targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
            var smoothTargetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, SmoothTime);
            var targetRotation = Quaternion.Euler(0f, smoothTargetRotation, 0f);
            transform.rotation = targetRotation;
        }

        public Vector3 LocalMoveDirection(Vector3 worldMoveDirection)
        {
            return transform.InverseTransformDirection(worldMoveDirection);
        }
    }
}