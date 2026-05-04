using AKD.AnimationEvents;
using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class ReloadState : AbstractCombatState
    {
        #region Constructor

        public ReloadState(CombatStateMachine stateMachine, CombatBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion

        private static readonly int ReloadId = Animator.StringToHash("Reload");
        private static readonly string ReloadEndEventName = "ReloadEnd";

        private int _reloadLayerId;

        public override void StateEnter()
        {
            _reloadLayerId = Blackboard.AnimationController.GetActiveLayerId();
            Blackboard.AnimationEventDispatcher.Register(_reloadLayerId, ReloadEndEventName, OnReloadAnimationEnd);
            Blackboard.AnimationController.SetTrigger(ReloadId);
            //TODO: When animation end change state to last
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            Blackboard.AnimationEventDispatcher.Unregister(_reloadLayerId, ReloadEndEventName, OnReloadAnimationEnd);
        }

        private void OnReloadAnimationEnd()
        {
            if (Blackboard.WeaponSlots.ActiveSlot?.Weapon)
            {
                var weapon = Blackboard.WeaponSlots.ActiveSlot.Weapon;
                if (weapon is Gun gun)
                {
                    gun.Reload();
                }
            }

            StateMachine.ReturnToPreviousState<IdleState>();
        }
    }
}