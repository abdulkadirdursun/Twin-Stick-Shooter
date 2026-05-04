using TwinStickShooter.InputSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class IdleState : AbstractCombatState
    {
        #region Constructor

        public IdleState(CombatStateMachine stateMachine, CombatBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion

        public override void StateEnter()
        {
            PlayerInputs.OnStartAiming += ChangeToAimedState;
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            PlayerInputs.OnStartAiming -= ChangeToAimedState;
        }

        private void ChangeToAimedState()
        {
            StateMachine.ChangeState<AimedState>();
        }
    }
}