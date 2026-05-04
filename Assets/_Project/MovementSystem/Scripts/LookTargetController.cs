using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.MovementSystem
{
    public class LookTargetController : MonoBehaviour
    {
        [SerializeField] private Transform cursorObject;
        [SerializeField] private float minDistanceClamp = 1.5f;
        [SerializeField, Range(1f, 100f)] private float sensitivity = 10;

        private bool _onMove;
        private Vector3 _movementInput;

        private void OnCursorPositionChanged(Vector2 input)
        {
            _onMove = input != Vector2.zero;
            _movementInput = new Vector3(input.x, 0f, input.y).normalized;
        }

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            PlayerInputs.LookTargetMovement += OnCursorPositionChanged;
        }

        private void Update()
        {
            if (!_onMove) return;
            var positionChange = _movementInput * (sensitivity * Time.deltaTime);
            var newPosition = cursorObject.localPosition + positionChange;
            if (newPosition.magnitude <= minDistanceClamp)
            {
                newPosition = newPosition.normalized * minDistanceClamp;
            }
            cursorObject.localPosition = newPosition;
        }

        private void OnDisable()
        {
            PlayerInputs.LookTargetMovement -= OnCursorPositionChanged;
        }

        #endregion
    }
}