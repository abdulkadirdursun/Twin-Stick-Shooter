using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventDispatcher : MonoBehaviour
    {
        private readonly struct EventKey : IEquatable<EventKey> {
            public readonly int StateHash;
            public readonly AnimationEventType EventType;
            public readonly float NormalizedTime;

            public EventKey(int stateHash, AnimationEventType eventType, float normalizedTime = 0f)
            {
                StateHash = stateHash;
                EventType = eventType;
                NormalizedTime = eventType == AnimationEventType.OnTime
                    ? Mathf.Round(normalizedTime * 1000f) / 1000f
                    : 0f;
            }

            public bool Equals(EventKey other)
            {
                return StateHash == other.StateHash
                       && EventType == other.EventType
                       && NormalizedTime == other.NormalizedTime;
            }

            public override bool Equals(object obj) => obj is EventKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = StateHash;
                    hash = (hash * 397) ^ (int)EventType;
                    hash = (hash * 397) ^ NormalizedTime.GetHashCode();
                    return hash;
                }
            }
        }
        

        private readonly Dictionary<EventKey, List<Action>> _callbacks = new();

        public void Register(int stateHash, AnimationEventType eventType, Action callback)
        {
            var key = new EventKey(stateHash, eventType);
            AddCallback(key, callback);
        }

        public void Unregister(int stateHash, AnimationEventType eventType, Action callback)
        {
            var key = new EventKey(stateHash, eventType);
            RemoveCallback(key, callback);
        }

        public void Register(int stateHash, float normalizedTime, Action callback)
        {
            var key = new EventKey(stateHash, AnimationEventType.OnTime, normalizedTime);
            AddCallback(key, callback);
        }

        public void Unregister(int stateHash, float normalizedTime, Action callback)
        {
            var key = new EventKey(stateHash, AnimationEventType.OnTime, normalizedTime);
            RemoveCallback(key, callback);
        }

        internal void Invoke(int stateHash, AnimationEventType eventType, float normalizedTime = 0f)
        {
            var key = new EventKey(stateHash, eventType, normalizedTime);
            if (!_callbacks.TryGetValue(key, out var list)) return;

            for (int i = 0; i < list.Count; i++)
            {
                list[i]?.Invoke();
            }
        }

        internal List<float> GetRegisteredTimes(int stateHash)
        {
            List<float> times = null;

            foreach (var key in _callbacks.Keys)
            {
                if (key.StateHash == stateHash && key.EventType == AnimationEventType.OnTime)
                {
                    times ??= new List<float>();
                    times.Add(key.NormalizedTime);
                }
            }

            return times;
        }

        private void AddCallback(EventKey key, Action callback)
        {
            if (!_callbacks.TryGetValue(key, out var list))
            {
                list = new List<Action>();
                _callbacks[key] = list;
            }

            if (!list.Contains(callback))
                list.Add(callback);
        }

        private void RemoveCallback(EventKey key, Action callback)
        {
            if (_callbacks.TryGetValue(key, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                    _callbacks.Remove(key);
            }
        }
    }
}
