using TwinStickShooter.StateMachineSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public abstract class AbstractCombatState : IState
    {
        #region Constructor

        protected AbstractCombatState(CombatStateMachine stateMachine, CombatBlackboard blackboard)
        {
            StateMachine = stateMachine;
            Blackboard = blackboard;
        }

        #endregion

        
        protected readonly CombatStateMachine StateMachine;
        protected readonly CombatBlackboard Blackboard;
        public abstract void StateEnter();
        public abstract void StateUpdate();
        public abstract void StateExit();
    }
}