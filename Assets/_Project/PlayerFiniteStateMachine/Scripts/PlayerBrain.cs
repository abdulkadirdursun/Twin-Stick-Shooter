using AKD.AnimationEvents;
using TwinStickShooter.AnimationSystem;
using TwinStickShooter.MovementSystem;
using TwinStickShooter.PlayerFiniteStateMachine.Combat;
using TwinStickShooter.PlayerFiniteStateMachine.Locomotion;
using TwinStickShooter.WeaponSlotSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine
{
    public class PlayerBrain : MonoBehaviour
    {
        [Header("Common Components")]
        [SerializeField] private AnimationController animationController;
        [Header("Locomotion Components")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private IKAimTargetPlacer ikAimTargetPlacer;
        [Header("Combat Components")]
        [SerializeField] private PlayerWeaponSlots playerWeaponSlots;
        [SerializeField] private AnimationEventDispatcher animationEventDispatcher;

        private LocomotionStateMachine _locomotionStateMachine;
        private CombatStateMachine _combatStateMachine;

        private void UpdateLocomotionState()
        {
            if (_locomotionStateMachine == null)
            {
                Debug.LogError("Locomotion State Machine is null!!!");
                return;
            }

            _locomotionStateMachine.CallStateUpdate();
        }

        private void UpdateCombatState()
        {
            if (_combatStateMachine == null)
            {
                Debug.LogError("Combat State Machine is null!!!");
                return;
            }

            _combatStateMachine.CallStateUpdate();
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            var locomotionBlackboard = new LocomotionBlackboard(movementController, animationController, ikAimTargetPlacer);
            _locomotionStateMachine = new LocomotionStateMachine(locomotionBlackboard);
            var combatBlackboard = new CombatBlackboard(animationController, playerWeaponSlots, animationEventDispatcher);
            _combatStateMachine = new CombatStateMachine(combatBlackboard);
        }

        private void Update()
        {
            UpdateLocomotionState();
            UpdateCombatState();
        }

        #endregion
    }
}