using System.Collections.Generic;
using UnityEngine;

namespace AKD.AnimationEvents
{
    /// <summary>
    /// Thin signal relay attached once per layer root state machine (use the dispatcher
    /// inspector's "Setup Controller" button). Forwards state lifecycle and threshold
    /// crossings to the object's <see cref="AnimationEventDispatcher"/>. Owns no gameplay logic.
    /// StateMachineBehaviour instances are per-Animator at runtime, so instance fields are safe.
    /// </summary>
    public class AnimationEventBehaviour : StateMachineBehaviour
    {
        private class ActiveState
        {
            public readonly List<float> Times = new();
            public readonly HashSet<float> Fired = new(); // keyed by rounded threshold value (stable across refreshes)
            public int StateHash;
            public int TagHash;
            public int LayerIndex;
            public bool Looping;
            public float LastCycleTime; // normalized within current cycle [0,1]
            public float LastRawTime;   // raw normalizedTime (grows past 1 on loops)
            public bool CompletedFired; // one-shot OnComplete latch
            public int Version;

            public void Reset()
            {
                Times.Clear();
                Fired.Clear();
                CompletedFired = false;
            }
        }

        private AnimationEventDispatcher _dispatcher;
        private bool _missingDispatcherLogged;

        // Keyed by (layerIndex, fullPathHash): cross-fades AND synced layers can have the same
        // fullPathHash active concurrently (synced layers share the source layer's states), so
        // fullPathHash alone is not unique. Entries are removed on exit (pooled).
        private readonly Dictionary<long, ActiveState> _activeStates = new();
        private static long StateKey(int layerIndex, int fullPathHash) => ((long)layerIndex << 32) | (uint)fullPathHash;
        private readonly Stack<ActiveState> _pool = new();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!TryGetDispatcher(animator)) return;

            // Mute decision is made once at entry: a state entered on a zero-weight layer raises
            // no events at all (no tracking entry → Update/Exit no-op), so Start/Exit never unpair.
            // Weight changes mid-state deliberately do not re-gate. Base layer is always active.
            if (layerIndex > 0
                && _dispatcher.IgnoreZeroWeightLayers
                && animator.GetLayerWeight(layerIndex) <= 0f)
            {
                return;
            }

            var state = _pool.Count > 0 ? _pool.Pop() : new ActiveState();
            state.Reset();
            state.StateHash = stateInfo.shortNameHash;
            state.TagHash = stateInfo.tagHash;
            state.LayerIndex = layerIndex;
            state.Looping = stateInfo.loop;
            state.LastRawTime = stateInfo.normalizedTime;
            state.LastCycleTime = state.Looping ? Frac(stateInfo.normalizedTime) : Mathf.Min(stateInfo.normalizedTime, 1f);
            RefreshTimes(state);

            _activeStates[StateKey(layerIndex, stateInfo.fullPathHash)] = state;

            _dispatcher.Invoke(new AnimationEventContext(animator, state.StateHash, state.TagHash,
                layerIndex, AnimationEventType.OnStart, state.LastCycleTime));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_dispatcher == null) return;
            if (!_activeStates.TryGetValue(StateKey(layerIndex, stateInfo.fullPathHash), out var state)) return;

            if (state.Version != _dispatcher.Version)
                RefreshTimes(state); // mid-state registrations take effect immediately

            float raw = stateInfo.normalizedTime;

            if (state.Looping && Mathf.FloorToInt(raw) > Mathf.FloorToInt(state.LastRawTime))
            {
                // Loop wrapped: finish the previous cycle (seam thresholds), complete it, reset.
                // If a frame spike skips multiple cycles, OnComplete fires once (not per cycle) and
                // OnTime thresholds inside fully-skipped middle cycles are not replayed — intended.
                FireThresholds(animator, state, state.LastCycleTime, 1f);
                _dispatcher.Invoke(new AnimationEventContext(animator, state.StateHash, state.TagHash,
                    layerIndex, AnimationEventType.OnComplete, 1f));
                state.Fired.Clear();
                state.LastCycleTime = 0f;
            }

            float current = state.Looping ? Frac(raw) : Mathf.Min(raw, 1f);
            FireThresholds(animator, state, state.LastCycleTime, current);

            if (!state.Looping && !state.CompletedFired && raw >= 1f)
            {
                state.CompletedFired = true;
                _dispatcher.Invoke(new AnimationEventContext(animator, state.StateHash, state.TagHash,
                    layerIndex, AnimationEventType.OnComplete, 1f));
            }

            state.LastCycleTime = current;
            state.LastRawTime = raw;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Cleanup must run even when the dispatcher was destroyed mid-state —
            // only the Invoke calls are guarded (plan-review finding: leak otherwise).
            if (!_activeStates.Remove(StateKey(layerIndex, stateInfo.fullPathHash), out var state)) return;

            if (_dispatcher != null)
            {
                // A one-shot that reached its end during the exit blend still completes.
                if (!state.Looping && !state.CompletedFired && stateInfo.normalizedTime >= 1f)
                {
                    state.CompletedFired = true;
                    _dispatcher.Invoke(new AnimationEventContext(animator, state.StateHash, state.TagHash,
                        layerIndex, AnimationEventType.OnComplete, 1f));
                }

                _dispatcher.Invoke(new AnimationEventContext(animator, state.StateHash, state.TagHash,
                    layerIndex, AnimationEventType.OnExit, state.LastCycleTime));
            }

            _pool.Push(state);
        }

        private void FireThresholds(Animator animator, ActiveState state, float from, float to)
        {
            // Fires thresholds t where from < t <= to. A threshold of exactly 0 therefore
            // never fires — that moment is OnStart (documented on AnimationEventType.OnTime).
            for (int i = 0; i < state.Times.Count; i++)
            {
                float threshold = state.Times[i];
                if (threshold <= from || threshold > to) continue;
                if (!state.Fired.Add(threshold)) continue;

                _dispatcher.Invoke(new AnimationEventContext(animator, state.StateHash, state.TagHash,
                    state.LayerIndex, AnimationEventType.OnTime, threshold));
            }
        }

        private void RefreshTimes(ActiveState state)
        {
            state.Times.Clear();
            _dispatcher.CollectTimes(state.StateHash, state.TagHash, state.LayerIndex, state.Times);
            state.Version = _dispatcher.Version;
        }

        private bool TryGetDispatcher(Animator animator)
        {
            if (_dispatcher != null) return true;
            if (animator.TryGetComponent(out _dispatcher)) return true;

            if (!_missingDispatcherLogged)
            {
                _missingDispatcherLogged = true;
                Debug.LogWarning($"<color=#FF5F5D>[AnimationEvents]</color> No AnimationEventDispatcher found on '{animator.gameObject.name}'.", animator);
            }

            return false;
        }

        private static float Frac(float value) => value - Mathf.Floor(value);
    }
}
