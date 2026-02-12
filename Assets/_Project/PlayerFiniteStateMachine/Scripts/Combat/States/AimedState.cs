using TwinStickShooter.InputSystem;
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
            PlayerInputs.OnStopAiming += ChangeToIdleState;
            Blackboard.AnimationController.SetBool(AimId, true);
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            Blackboard.AnimationController.SetBool(AimId, false);
            PlayerInputs.OnStopAiming -= ChangeToIdleState;
        }

        private void ChangeToIdleState()
        {
            StateMachine.ChangeState<IdleState>();
        }
    }
}