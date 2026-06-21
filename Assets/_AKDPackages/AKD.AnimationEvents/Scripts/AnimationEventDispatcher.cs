using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    /// <summary>
    /// Per-object hub owning animation-event registrations. <see cref="AnimationEventBehaviour"/>
    /// forwards state lifecycle into <see cref="Invoke"/>, which fans out to every registration
    /// whose scope matches: (state | tag | any) x (exact layer | any layer) — at most 6 key probes.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationEventDispatcher : MonoBehaviour
    {
        [Tooltip("When on, states entered while their layer weight is 0 raise no events for that state instance. " +
                 "Base layer (index 0) is always active. Turn off for zero-weight logic-layer setups.")]
        [SerializeField] private bool ignoreZeroWeightLayers = true;

        /// <summary>When true, states entered on a zero-weight layer (index > 0) are fully muted for that instance.</summary>
        internal bool IgnoreZeroWeightLayers => ignoreZeroWeightLayers;

        private readonly struct EventKey : IEquatable<EventKey>
        {
            public readonly AnimationEventScope Scope;
            public readonly AnimationEventType EventType;
            public readonly float NormalizedTime; // rounded; 0 unless OnTime

            public EventKey(AnimationEventScope scope, AnimationEventType eventType, float normalizedTime = 0f)
            {
                Scope = scope;
                EventType = eventType;
                NormalizedTime = eventType == AnimationEventType.OnTime ? RoundTime(normalizedTime) : 0f;
            }

            public bool Equals(EventKey other)
                => Scope.Equals(other.Scope)
                   && EventType == other.EventType
                   && NormalizedTime.Equals(other.NormalizedTime);

            public override bool Equals(object obj) => obj is EventKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = Scope.GetHashCode();
                    hash = (hash * 397) ^ (int)EventType;
                    hash = (hash * 397) ^ NormalizedTime.GetHashCode();
                    return hash;
                }
            }
        }

        private readonly Dictionary<EventKey, List<Action<AnimationEventContext>>> _callbacks = new();

        // Dispatch snapshot so callbacks may register/unregister (incl. themselves) mid-invoke.
        // A single shared buffer is safe: animator updates are not re-entrant, so Invoke never nests.
        private readonly List<Action<AnimationEventContext>> _scratch = new();

        /// <summary>Bumped on every register/unregister; lets the behaviour refresh cached thresholds mid-state.</summary>
        internal int Version { get; private set; }

        /// <summary>Register for OnStart / OnComplete / OnExit. For OnTime use the normalizedTime overload.</summary>
        public void Register(AnimationEventScope scope, AnimationEventType eventType, Action<AnimationEventContext> callback)
        {
            if (eventType == AnimationEventType.OnTime)
            {
                Debug.LogWarning("<color=#FF5F5D>[AnimationEvents]</color> OnTime registrations need a normalizedTime — use Register(scope, normalizedTime, callback).", this);
                return;
            }

            AddCallback(new EventKey(scope, eventType), callback);
        }

        /// <summary>Register an OnTime callback firing when the state's cycle time crosses normalizedTime (0..1].</summary>
        public void Register(AnimationEventScope scope, float normalizedTime, Action<AnimationEventContext> callback)
            => AddCallback(new EventKey(scope, AnimationEventType.OnTime, normalizedTime), callback);

        public void Unregister(AnimationEventScope scope, AnimationEventType eventType, Action<AnimationEventContext> callback)
            => RemoveCallback(new EventKey(scope, eventType), callback);

        public void Unregister(AnimationEventScope scope, float normalizedTime, Action<AnimationEventContext> callback)
            => RemoveCallback(new EventKey(scope, AnimationEventType.OnTime, normalizedTime), callback);

        internal void Invoke(in AnimationEventContext context)
        {
            _scratch.Clear();

            int layer = context.LayerIndex;
            Collect(new EventKey(AnimationEventScope.State(context.StateHash, layer), context.EventType, context.NormalizedTime));
            Collect(new EventKey(AnimationEventScope.State(context.StateHash), context.EventType, context.NormalizedTime));

            if (context.TagHash != 0)
            {
                Collect(new EventKey(AnimationEventScope.Tag(context.TagHash, layer), context.EventType, context.NormalizedTime));
                Collect(new EventKey(AnimationEventScope.Tag(context.TagHash), context.EventType, context.NormalizedTime));
            }

            Collect(new EventKey(AnimationEventScope.Any(layer), context.EventType, context.NormalizedTime));
            Collect(new EventKey(AnimationEventScope.Any(), context.EventType, context.NormalizedTime));

            for (int i = 0; i < _scratch.Count; i++)
            {
                _scratch[i]?.Invoke(context);
            }

            _scratch.Clear();
        }

        /// <summary>
        /// Fills <paramref name="buffer"/> with the distinct, sorted OnTime thresholds whose scope
        /// matches the given state/tag/layer. The behaviour owns and reuses the buffer.
        /// </summary>
        internal void CollectTimes(int stateHash, int tagHash, int layerIndex, List<float> buffer)
        {
            foreach (var pair in _callbacks)
            {
                var key = pair.Key;
                if (key.EventType != AnimationEventType.OnTime) continue;
                if (!ScopeMatches(key.Scope, stateHash, tagHash, layerIndex)) continue;
                if (!buffer.Contains(key.NormalizedTime))
                    buffer.Add(key.NormalizedTime);
            }

            buffer.Sort();
        }

        internal static float RoundTime(float time) => Mathf.Round(time * 1000f) / 1000f;

        private static bool ScopeMatches(in AnimationEventScope scope, int stateHash, int tagHash, int layerIndex)
        {
            if (scope.Layer != AnimationEventScope.AnyLayer && scope.Layer != layerIndex) return false;

            return scope.Kind switch
            {
                AnimationEventScopeKind.State => scope.Hash == stateHash,
                AnimationEventScopeKind.Tag => tagHash != 0 && scope.Hash == tagHash,
                _ => true,
            };
        }

        private void Collect(in EventKey key)
        {
            if (!_callbacks.TryGetValue(key, out var list)) return;

            for (int i = 0; i < list.Count; i++)
            {
                _scratch.Add(list[i]);
            }
        }

        private void AddCallback(in EventKey key, Action<AnimationEventContext> callback)
        {
            if (callback == null) return;

            if (!_callbacks.TryGetValue(key, out var list))
            {
                list = new List<Action<AnimationEventContext>>();
                _callbacks[key] = list;
            }

            if (list.Contains(callback)) return;

            list.Add(callback);
            Version++;
        }

        private void RemoveCallback(in EventKey key, Action<AnimationEventContext> callback)
        {
            if (!_callbacks.TryGetValue(key, out var list)) return;
            if (!list.Remove(callback)) return;

            if (list.Count == 0)
                _callbacks.Remove(key);

            Version++;
        }
    }
}
