using TwinStickShooter.MovementSystem.FSM;
using UnityEngine;

namespace TwinStickShooter.MovementSystem
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 180f;

        private PlayerMovementStateMachine _playerMovementStateMachine;

        #region MonoBehaviour Methods

        private void Awake()
        {
            var blackboard = new PlayerMovementBlackboard(characterController, moveSpeed, rotateSpeed);
            _playerMovementStateMachine = new PlayerMovementStateMachine(blackboard);
        }

        private void Update()
        {
            if (_playerMovementStateMachine == null)
            {
                Debug.LogError("Player Movement State Machine is null!!!");
                return;
            }

            _playerMovementStateMachine.CallStateUpdate();
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