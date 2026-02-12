using System;
using System.Collections.Generic;
using TwinStickShooter.StateMachineSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class CombatStateMachine : StateMachine<AbstractCombatState>
    {
        public CombatStateMachine(CombatBlackboard blackboard)
        {
            AvailableStates = new Dictionary<Type, AbstractCombatState>
            {
                { typeof(IdleState), new IdleState(this, blackboard) },
                { typeof(AimedState), new AimedState(this, blackboard) },
                { typeof(ReloadState), new ReloadState(this, blackboard) }
            };

            ChangeState<IdleState>();
        }
    }
}