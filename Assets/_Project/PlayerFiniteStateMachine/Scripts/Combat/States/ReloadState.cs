using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class ReloadState:AbstractCombatState
    {
        #region Constructor

        public ReloadState(CombatStateMachine stateMachine, CombatBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion
        
        private static readonly int ReloadId = Animator.StringToHash("Reload");

        public override void StateEnter()
        {
            Blackboard.AnimationController.SetTrigger(ReloadId);
            //TODO: When animation end change state to last
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
        }
    }
}