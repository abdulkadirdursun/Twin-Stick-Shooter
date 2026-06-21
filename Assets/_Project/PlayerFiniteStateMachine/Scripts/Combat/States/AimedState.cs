using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class AimedState : AbstractCombatState
    {
        #region Constructor

        public AimedState(CombatStateMachine stateMachine, CombatBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion

        private static readonly int AimId = Animator.StringToHash("Aim");

        public override void StateEnter()
        {
            Blackboard.AnimationController.SetBool(AimId, true);
            Blackboard.WeaponController.ChangeAttackPermit(true);
            Blackboard.GameplayInputs.StopAiming += ChangeToIdleState;
            Blackboard.GameplayInputs.Reload += ChangeToReloadState;
        }

        public override void StateUpdate()
        {
            //TODO: How to detect the reload state?
        }

        public override void StateExit()
        {
            Blackboard.AnimationController.SetBool(AimId, false);
            Blackboard.WeaponController.ChangeAttackPermit(false);
            Blackboard.GameplayInputs.StopAiming -= ChangeToIdleState;
            Blackboard.GameplayInputs.Reload -= ChangeToReloadState;
        }

        private void ChangeToIdleState()
        {
            StateMachine.ChangeState<IdleState>();
        }
        
        private void ChangeToReloadState()
        {
            StateMachine.ChangeState<ReloadState>();
        }
    }
}