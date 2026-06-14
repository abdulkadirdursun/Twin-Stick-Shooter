using System;
using UnityEngine;
using UnityEngine.Events;

namespace AKD.AnimationEvents
{
    /// <summary>
    /// Designer-authored event binding consumed by <see cref="AnimationEventListener"/>.
    /// Public serialized fields by design: this is a data carrier edited via SerializedProperty.
    /// </summary>
    [Serializable]
    public struct AnimationEventBinding
    {
        public AnimationEventScopeKind scopeKind;

        // State scope (stateName kept for editor display + stale detection)
        public string stateName;
        public int stateHash;

        // Tag scope (hashed at runtime; name kept for editor display + stale detection)
        public string tagName;

        /// <summary>AnimationEventScope.AnyLayer (-1) = any layer; otherwise a layer index.</summary>
        public int layerIndex;

        public AnimationEventType eventType;

        // Min 0.001: a threshold of exactly 0 never fires (that moment is OnStart) —
        // the range guard prevents designers authoring a silently dead binding.
        [Range(0.001f, 1f)]
        public float normalizedTime;

        public UnityEvent response;

        public AnimationEventScope ToScope() => scopeKind switch
        {
            AnimationEventScopeKind.State => AnimationEventScope.State(stateHash, layerIndex),
            AnimationEventScopeKind.Tag => AnimationEventScope.Tag(tagName ?? string.Empty, layerIndex),
            _ => AnimationEventScope.Any(layerIndex),
        };
    }
}
