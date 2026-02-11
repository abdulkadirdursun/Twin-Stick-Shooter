using TwinStickShooter.MovementSystem;
using TwinStickShooter.PlayerFiniteStateMachine.Locomotion;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine
{
    public class PlayerBrain : MonoBehaviour
    {
        [Header("Locomotion")]
        [SerializeField] private MovementController movementController;

        private LocomotionStateMachine _locomotionStateMachine;

        #region MonoBehaviour Methods

        private void Awake()
        {
            _locomotionStateMachine = new LocomotionStateMachine(movementController);
        }

        private void Update()
        {
            if (_locomotionStateMachine == null)
            {
                Debug.LogError("Locomotion State Machine is null!!!");
                return;
            }

            _locomotionStateMachine.CallStateUpdate();
        }

        #endregion
    }
}