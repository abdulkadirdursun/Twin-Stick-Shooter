using System;
using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerControllers
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;

        private Vector3 _moveDirection;

        private void SetMoveInput(Vector2 moveInput)
        {
            _moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        }

        private void Move()
        {
            var movement = _moveDirection * (moveSpeed * Time.deltaTime);
            characterController.Move(movement);
        }

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            PlayerInputs.OnMove += SetMoveInput;
        }

        private void Update()
        {
            if (_moveDirection == Vector3.zero) return;
            Move();
        }

        private void OnDisable()
        {
            PlayerInputs.OnMove -= SetMoveInput;
        }

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
        }
#endif
    }
}