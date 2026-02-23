using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    public class AnimationEventBehaviour : StateMachineBehaviour
    {
        [SerializeField] private List<AnimationEventEntry> _events = new();

        public IReadOnlyList<AnimationEventEntry> Events => _events;

        private class StateTracker
        {
            public AnimationEventDispatcher Dispatcher;
            public float LastNormalizedTime;
            public readonly HashSet<int> InvokedThisCycle = new();
        }

        private readonly Dictionary<int, StateTracker> _trackers = new();

        private StateTracker GetOrCreateTracker(Animator animator)
        {
            int id = animator.GetInstanceID();
            if (!_trackers.TryGetValue(id, out var tracker))
            {
                tracker = new StateTracker();
                _trackers[id] = tracker;
            }

            return tracker;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var tracker = GetOrCreateTracker(animator);

            if (tracker.Dispatcher == null)
            {
                if (!animator.TryGetComponent(out tracker.Dispatcher))
                {
                    Debug.LogWarning(
                        $"<color=#FF5F5D>[AnimationEvents]</color> No AnimationEventDispatcher found on '{animator.gameObject.name}'.");
                    return;
                }
            }

            tracker.LastNormalizedTime = 0f;
            tracker.InvokedThisCycle.Clear();

            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].eventType == AnimationEventType.OnEnter)
                    tracker.Dispatcher.Fire(layerIndex, _events[i].eventName);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var tracker = GetOrCreateTracker(animator);
            if (tracker.Dispatcher == null) return;

            float currentTime = stateInfo.normalizedTime % 1f;

            if (currentTime < tracker.LastNormalizedTime)
                tracker.InvokedThisCycle.Clear();

            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].eventType != AnimationEventType.OnTime) continue;
                if (tracker.InvokedThisCycle.Contains(i)) continue;

                float threshold = _events[i].normalizedTime;
                if (tracker.LastNormalizedTime < threshold && currentTime >= threshold)
                {
                    tracker.InvokedThisCycle.Add(i);
                    tracker.Dispatcher.Fire(layerIndex, _events[i].eventName);
                }
            }

            tracker.LastNormalizedTime = currentTime;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var tracker = GetOrCreateTracker(animator);
            if (tracker.Dispatcher == null) return;

            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].eventType == AnimationEventType.OnExit)
                    tracker.Dispatcher.Fire(layerIndex, _events[i].eventName);
            }

            tracker.InvokedThisCycle.Clear();
        }
    }
}
