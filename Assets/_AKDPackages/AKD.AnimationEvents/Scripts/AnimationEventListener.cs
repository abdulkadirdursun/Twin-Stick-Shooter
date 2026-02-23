using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    public class AnimationEventListener : MonoBehaviour
    {
        [SerializeField] private AnimationEventDispatcher _dispatcher;
        [SerializeField] private List<AnimationEventBinding> _bindings = new();

        private Action[] _cachedCallbacks;

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            if (_dispatcher == null)
            {
                Debug.LogWarning(
                    $"<color=#FF5F5D>[AnimationEvents]</color> AnimationEventListener on '{gameObject.name}' has no dispatcher assigned.");
                return;
            }

            _cachedCallbacks = new Action[_bindings.Count];

            for (int i = 0; i < _bindings.Count; i++)
            {
                var binding = _bindings[i];
                int index = i;
                _cachedCallbacks[index] = () => binding.response?.Invoke();

                _dispatcher.Register(binding.layerIndex, binding.eventName, _cachedCallbacks[index]);
            }
        }

        private void OnDisable()
        {
            if (_dispatcher == null || _cachedCallbacks == null) return;

            for (int i = 0; i < _bindings.Count; i++)
            {
                if (i >= _cachedCallbacks.Length || _cachedCallbacks[i] == null) continue;

                var binding = _bindings[i];
                _dispatcher.Unregister(binding.layerIndex, binding.eventName, _cachedCallbacks[i]);
            }

            _cachedCallbacks = null;
        }

        #endregion
    }
}
