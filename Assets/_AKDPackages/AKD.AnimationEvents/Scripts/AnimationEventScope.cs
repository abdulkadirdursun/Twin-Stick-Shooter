using System;
using UnityEngine;

namespace AKD.AnimationEvents
{
    /// <summary>What a registration listens to: a specific state, a state tag, or every state.</summary>
    public enum AnimationEventScopeKind
    {
        State,
        Tag,
        Any
    }

    /// <summary>
    /// Registration scope for animation events: what (state / tag / any) x where (specific layer / any layer).
    /// Create instances via the static factory methods. String overloads hash internally
    /// (Animator.StringToHash); use the int-hash overloads with cached hashes in hot paths.
    /// </summary>
    public readonly struct AnimationEventScope : IEquatable<AnimationEventScope>
    {
        /// <summary>Layer wildcard — the scope matches the same state/tag on every layer.</summary>
        public const int AnyLayer = -1;

        public readonly AnimationEventScopeKind Kind;

        /// <summary>State or tag name hash. 0 when <see cref="Kind"/> is Any.</summary>
        public readonly int Hash;

        /// <summary><see cref="AnyLayer"/> or a concrete layer index.</summary>
        public readonly int Layer;

        private AnimationEventScope(AnimationEventScopeKind kind, int hash, int layer)
        {
            Kind = kind;
            Hash = hash;
            Layer = layer;
        }

        /// <summary>
        /// Scope targeting one state by its SHORT name (e.g. "Idle", never "Base Layer.Idle" —
        /// a full-path string hashes to fullPathHash and silently never matches). Any layer unless specified.
        /// </summary>
        public static AnimationEventScope State(string stateName, int layer = AnyLayer)
            => new(AnimationEventScopeKind.State, Animator.StringToHash(stateName), layer);

        /// <summary>Scope targeting one state by pre-hashed short name (shortNameHash, not fullPathHash).</summary>
        public static AnimationEventScope State(int stateHash, int layer = AnyLayer)
            => new(AnimationEventScopeKind.State, stateHash, layer);

        /// <summary>Scope targeting every state carrying the given tag — must match the Tag field on the state in the Animator inspector exactly (case-sensitive).</summary>
        public static AnimationEventScope Tag(string tag, int layer = AnyLayer)
            => new(AnimationEventScopeKind.Tag, Animator.StringToHash(tag), layer);

        /// <summary>Scope targeting every state carrying the given pre-hashed tag.</summary>
        public static AnimationEventScope Tag(int tagHash, int layer = AnyLayer)
            => new(AnimationEventScopeKind.Tag, tagHash, layer);

        /// <summary>Scope targeting every state (optionally restricted to one layer).</summary>
        public static AnimationEventScope Any(int layer = AnyLayer)
            => new(AnimationEventScopeKind.Any, 0, layer);

        public bool Equals(AnimationEventScope other)
            => Kind == other.Kind && Hash == other.Hash && Layer == other.Layer;

        public override bool Equals(object obj) => obj is AnimationEventScope other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)Kind;
                hash = (hash * 397) ^ Hash;
                hash = (hash * 397) ^ Layer;
                return hash;
            }
        }
    }
}
