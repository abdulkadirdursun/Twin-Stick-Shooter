using System;
using System.Collections.Generic;
using TwinStickShooter.StateMachineSystem;

namespace TwinStickShooter.MovementSystem.FSM
{
    public sealed class PlayerMovementStateMachine : StateMachine<BaseMovementState>
    {
        #region Constructor

        public PlayerMovementStateMachine(PlayerMovementBlackboard blackboard)
        {
            _blackboard = blackboard;
            InitializeStateMachine();
        }

        #endregion

        private readonly PlayerMovementBlackboard _blackboard;

        public override void InitializeStateMachine()
        {
            AvailableStates = new Dictionary<Type, BaseMovementState>
            {
                { typeof(FreeMovementState), new FreeMovementState(this, _blackboard) },
                { typeof(AimedMovementState), new AimedMovementState(this, _blackboard) }
            };

            ChangeState<FreeMovementState>();
        }
    }
}