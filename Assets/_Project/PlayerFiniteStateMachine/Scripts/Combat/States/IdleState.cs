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
            Blackboard.GameplayInputs.StartAiming += ChangeToAimedState;
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            Blackboard.GameplayInputs.StartAiming -= ChangeToAimedState;
        }

        private void ChangeToAimedState()
        {
            StateMachine.ChangeState<AimedState>();
        }
    }
}