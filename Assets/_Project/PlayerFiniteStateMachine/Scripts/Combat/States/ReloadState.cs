using AKD.AnimationEvents;
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

        private int _reloadLayerId;

        public override void StateEnter()
        {
            Blackboard.AnimationController.SetTrigger(ReloadId);
            Blackboard.AnimationEventDispatcher.Register(AnimationEventScope.State(ReloadId), AnimationEventType.OnComplete, OnReloadAnimationEnd);
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            Blackboard.AnimationEventDispatcher.Unregister(AnimationEventScope.State(ReloadId), AnimationEventType.OnComplete, OnReloadAnimationEnd);
        }

        private void OnReloadAnimationEnd(AnimationEventContext context)
        {
            Debug.LogWarning("[ReloadState] OnReloadAnimationEnd");
            StateMachine.ChangeState<IdleState>();
        }
    }
}