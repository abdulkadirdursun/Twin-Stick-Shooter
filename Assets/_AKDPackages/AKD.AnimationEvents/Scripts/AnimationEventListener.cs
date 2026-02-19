using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    public class AnimationEventListener : MonoBehaviour
    {
        [SerializeField] private AnimationEventDispatcher dispatcher;
        [SerializeField] private List<AnimationEventBinding> bindings = new();

        private Action[] _cachedCallbacks;

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            if (dispatcher == null)
            {
                Debug.LogWarning($"<color=#FF5F5D>[AnimationEvents]</color> AnimationEventListener on '{gameObject.name}' has no dispatcher assigned.");
                return;
            }

            _cachedCallbacks = new Action[bindings.Count];

            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                int index = i;
                _cachedCallbacks[index] = () => binding.response?.Invoke();

                if (binding.eventType == AnimationEventType.OnTime)
                    dispatcher.Register(binding.stateHash, binding.normalizedTime, _cachedCallbacks[index]);
                else
                    dispatcher.Register(binding.stateHash, binding.eventType, _cachedCallbacks[index]);
            }
        }

        private void OnDisable()
        {
            if (dispatcher == null || _cachedCallbacks == null) return;

            for (int i = 0; i < bindings.Count; i++)
            {
                if (i >= _cachedCallbacks.Length || _cachedCallbacks[i] == null) continue;

                var binding = bindings[i];

                if (binding.eventType == AnimationEventType.OnTime)
                    dispatcher.Unregister(binding.stateHash, binding.normalizedTime, _cachedCallbacks[i]);
                else
                    dispatcher.Unregister(binding.stateHash, binding.eventType, _cachedCallbacks[i]);
            }

            _cachedCallbacks = null;
        }

        #endregion
    }
}
