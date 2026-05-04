using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventDispatcher : MonoBehaviour
    {
        private readonly struct EventKey : IEquatable<EventKey>
        {
            public readonly int LayerIndex;
            public readonly string EventName;

            public EventKey(int layerIndex, string eventName)
            {
                LayerIndex = layerIndex;
                EventName = eventName;
            }

            public bool Equals(EventKey other)
            {
                return LayerIndex == other.LayerIndex
                       && string.Equals(EventName, other.EventName, StringComparison.Ordinal);
            }

            public override bool Equals(object obj) => obj is EventKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return (LayerIndex * 397) ^ (EventName != null ? StringComparer.Ordinal.GetHashCode(EventName) : 0);
                }
            }
        }

        private readonly Dictionary<EventKey, List<Action>> _callbacks = new();

        public void Register(int layerIndex, string eventName, Action callback)
        {
            var key = new EventKey(layerIndex, eventName);
            if (!_callbacks.TryGetValue(key, out var list))
            {
                list = new List<Action>();
                _callbacks[key] = list;
            }

            if (!list.Contains(callback))
                list.Add(callback);
        }

        public void Unregister(int layerIndex, string eventName, Action callback)
        {
            var key = new EventKey(layerIndex, eventName);
            if (_callbacks.TryGetValue(key, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                    _callbacks.Remove(key);
            }
        }

        internal void Fire(int layerIndex, string eventName)
        {
            var key = new EventKey(layerIndex, eventName);
            if (!_callbacks.TryGetValue(key, out var list)) return;

            for (int i = 0; i < list.Count; i++)
            {
                list[i]?.Invoke();
            }
        }
    }
}
