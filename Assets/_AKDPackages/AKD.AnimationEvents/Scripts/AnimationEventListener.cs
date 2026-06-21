using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    /// <summary>Bridges designer-authored <see cref="AnimationEventBinding"/>s to the dispatcher at runtime.</summary>
    public class AnimationEventListener : MonoBehaviour
    {
        [SerializeField] private AnimationEventDispatcher dispatcher;
        [SerializeField] private List<AnimationEventBinding> bindings = new();

        private Action<AnimationEventContext>[] _cachedCallbacks;

        private void OnEnable()
        {
            if (dispatcher == null)
            {
                Debug.LogWarning($"<color=#FF5F5D>[AnimationEvents]</color> AnimationEventListener on '{gameObject.name}' has no dispatcher assigned.", this);
                return;
            }

            _cachedCallbacks = new Action<AnimationEventContext>[bindings.Count];

            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                _cachedCallbacks[i] = _ => binding.response?.Invoke();

                var scope = binding.ToScope();
                if (binding.eventType == AnimationEventType.OnTime)
                    dispatcher.Register(scope, binding.normalizedTime, _cachedCallbacks[i]);
                else
                    dispatcher.Register(scope, binding.eventType, _cachedCallbacks[i]);
            }
        }

        private void OnDisable()
        {
            if (dispatcher == null || _cachedCallbacks == null) return;

            for (int i = 0; i < bindings.Count && i < _cachedCallbacks.Length; i++)
            {
                if (_cachedCallbacks[i] == null) continue;

                var binding = bindings[i];
                var scope = binding.ToScope();

                if (binding.eventType == AnimationEventType.OnTime)
                    dispatcher.Unregister(scope, binding.normalizedTime, _cachedCallbacks[i]);
                else
                    dispatcher.Unregister(scope, binding.eventType, _cachedCallbacks[i]);
            }

            _cachedCallbacks = null;
        }
    }
}
