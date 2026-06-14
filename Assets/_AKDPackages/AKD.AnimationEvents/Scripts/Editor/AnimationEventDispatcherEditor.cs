using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AKD.AnimationEvents.Editor
{
    [CustomEditor(typeof(AnimationEventDispatcher))]
    public class AnimationEventDispatcherEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var controller = GetController((AnimationEventDispatcher)target);
            if (controller == null)
            {
                EditorGUILayout.HelpBox("No AnimatorController assigned to the Animator.", MessageType.Info);
                return;
            }

            bool relayOnAllRoots = HasBehaviourOnAllLayerRoots(controller);
            bool hasStrays = HasStrayStateBehaviours(controller);

            if (!relayOnAllRoots)
            {
                EditorGUILayout.HelpBox(
                    "One or more layers have no AnimationEventBehaviour on their root state machine — events on those layers will never fire. Click Setup Controller.",
                    MessageType.Warning);
            }
            else if (hasStrays)
            {
                EditorGUILayout.HelpBox(
                    "Controller has per-state AnimationEventBehaviours (legacy setup) — Setup Controller removes them; the root relay already covers those states.",
                    MessageType.Warning);
            }

            if (GUILayout.Button("Setup Controller"))
            {
                SetupController(controller);
            }
        }

        private static AnimatorController GetController(AnimationEventDispatcher dispatcher)
        {
            var animator = dispatcher.GetComponent<Animator>();
            return animator != null ? animator.runtimeAnimatorController as AnimatorController : null;
        }

        private static bool HasBehaviourOnAllLayerRoots(AnimatorController controller)
        {
            foreach (var layer in controller.layers)
            {
                if (layer.syncedLayerIndex >= 0) continue; // synced layers reuse the source layer's state machine

                bool found = false;
                foreach (var behaviour in layer.stateMachine.behaviours)
                {
                    if (behaviour is AnimationEventBehaviour) { found = true; break; }
                }

                if (!found) return false;
            }

            return true;
        }

        private static bool HasStrayStateBehaviours(AnimatorController controller)
        {
            foreach (var layer in controller.layers)
            {
                if (layer.syncedLayerIndex >= 0) continue;
                if (HasStateBehaviours(layer.stateMachine)) return true;
            }

            return false;
        }

        private static bool HasStateBehaviours(AnimatorStateMachine stateMachine)
        {
            foreach (var childState in stateMachine.states)
            {
                foreach (var behaviour in childState.state.behaviours)
                {
                    if (behaviour is AnimationEventBehaviour) return true;
                }
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                if (HasStateBehaviours(childMachine.stateMachine)) return true;
            }

            return false;
        }

        private static void SetupController(AnimatorController controller)
        {
            Undo.RegisterCompleteObjectUndo(controller, "Setup AnimationEvents Controller");

            foreach (var layer in controller.layers)
            {
                if (layer.syncedLayerIndex >= 0) continue;

                bool hasRelay = false;
                foreach (var behaviour in layer.stateMachine.behaviours)
                {
                    if (behaviour is AnimationEventBehaviour) { hasRelay = true; break; }
                }

                if (!hasRelay)
                    layer.stateMachine.AddStateMachineBehaviour<AnimationEventBehaviour>();

                RemoveStateBehaviours(layer.stateMachine);
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#5DFF8B>[AnimationEvents]</color> '{controller.name}' set up: relay on every layer root, per-state duplicates removed.", controller);
        }

        private static void RemoveStateBehaviours(AnimatorStateMachine stateMachine)
        {
            foreach (var childState in stateMachine.states)
            {
                var state = childState.state;
                var behaviours = state.behaviours;
                var kept = new List<StateMachineBehaviour>(behaviours.Length);
                var removed = new List<StateMachineBehaviour>();

                foreach (var behaviour in behaviours)
                {
                    if (behaviour is AnimationEventBehaviour) removed.Add(behaviour);
                    else kept.Add(behaviour);
                }

                if (removed.Count == 0) continue;

                // AnimatorState is a separate sub-asset — the controller-level undo registration
                // does NOT capture it (plan-review finding). Record it explicitly, and use the
                // undo-aware destroy so Ctrl+Z restores the removed behaviours.
                Undo.RecordObject(state, "Setup AnimationEvents Controller");
                state.behaviours = kept.ToArray();
                foreach (var behaviour in removed)
                {
                    if (behaviour != null)
                        Undo.DestroyObjectImmediate(behaviour);
                }
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                RemoveStateBehaviours(childMachine.stateMachine);
            }
        }
    }
}
