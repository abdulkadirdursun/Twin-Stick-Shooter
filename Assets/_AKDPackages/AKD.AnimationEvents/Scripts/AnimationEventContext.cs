using UnityEngine;

namespace AKD.AnimationEvents
{
    /// <summary>
    /// Describes which state raised an animation event. Passed to every callback so
    /// wildcard (tag / any) subscribers can tell exactly what fired.
    /// </summary>
    public readonly struct AnimationEventContext
    {
        public readonly Animator Animator;

        /// <summary>shortNameHash of the state that raised the event.</summary>
        public readonly int StateHash;

        /// <summary>tagHash of the state (0 when untagged).</summary>
        public readonly int TagHash;

        public readonly int LayerIndex;
        public readonly AnimationEventType EventType;

        /// <summary>
        /// Cycle-normalized time at dispatch: the crossed threshold for OnTime, 1 for OnComplete,
        /// the entry/exit time for OnStart/OnExit.
        /// </summary>
        public readonly float NormalizedTime;

        public AnimationEventContext(Animator animator, int stateHash, int tagHash, int layerIndex,
            AnimationEventType eventType, float normalizedTime)
        {
            Animator = animator;
            StateHash = stateHash;
            TagHash = tagHash;
            LayerIndex = layerIndex;
            EventType = eventType;
            NormalizedTime = normalizedTime;
        }
    }
}
