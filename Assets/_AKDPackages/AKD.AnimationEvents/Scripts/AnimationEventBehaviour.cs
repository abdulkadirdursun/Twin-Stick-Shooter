using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    public class AnimationEventBehaviour : StateMachineBehaviour
    {
        private class PerAnimatorState
        {
            public AnimationEventDispatcher Dispatcher;
            public List<float> RegisteredTimes;
            public readonly HashSet<int> InvokedThisCycle = new();
            public float LastNormalizedTime;
        }

        private readonly Dictionary<int, PerAnimatorState> _stateByAnimator = new();

        private PerAnimatorState GetOrCreateState(Animator animator)
        {
            int id = animator.GetInstanceID();
            if (!_stateByAnimator.TryGetValue(id, out var state))
            {
                state = new PerAnimatorState();
                _stateByAnimator[id] = state;
            }

            return state;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var state = GetOrCreateState(animator);

            if (state.Dispatcher == null)
            {
                if (!animator.TryGetComponent(out state.Dispatcher))
                {
                    Debug.LogWarning(
                        $"<color=#FF5F5D>[AnimationEvents]</color> No AnimationEventDispatcher found on '{animator.gameObject.name}'.");
                    return;
                }
            }

            state.LastNormalizedTime = 0f;
            state.InvokedThisCycle.Clear();
            state.RegisteredTimes = state.Dispatcher.GetRegisteredTimes(stateInfo.shortNameHash);

            state.Dispatcher.Invoke(stateInfo.shortNameHash, AnimationEventType.OnStart);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var state = GetOrCreateState(animator);
            if (state.Dispatcher == null || state.RegisteredTimes == null) return;

            float currentTime = stateInfo.normalizedTime % 1f;

            if (currentTime < state.LastNormalizedTime)
            {
                state.InvokedThisCycle.Clear();
            }

            int stateHash = stateInfo.shortNameHash;

            for (int i = 0; i < state.RegisteredTimes.Count; i++)
            {
                if (state.InvokedThisCycle.Contains(i)) continue;

                float threshold = state.RegisteredTimes[i];
                if (state.LastNormalizedTime < threshold && currentTime >= threshold)
                {
                    state.InvokedThisCycle.Add(i);
                    state.Dispatcher.Invoke(stateHash, AnimationEventType.OnTime, threshold);
                }
            }

            state.LastNormalizedTime = currentTime;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var state = GetOrCreateState(animator);
            if (state.Dispatcher == null) return;

            state.Dispatcher.Invoke(stateInfo.shortNameHash, AnimationEventType.OnEnd);
            state.InvokedThisCycle.Clear();
            state.RegisteredTimes = null;
        }
    }
}
