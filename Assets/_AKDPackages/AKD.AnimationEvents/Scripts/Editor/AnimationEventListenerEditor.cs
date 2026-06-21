using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AKD.AnimationEvents.Editor
{
    [CustomEditor(typeof(AnimationEventListener))]
    public class AnimationEventListenerEditor : UnityEditor.Editor
    {
        private sealed class StateEntry
        {
            public string DisplayPath; // "LayerName/StateName"
            public string StateName;
            public int StateHash;
            public int LayerIndex;
            public AnimationClip Clip;
        }

        private SerializedProperty _dispatcher;
        private SerializedProperty _bindings;

        private readonly List<StateEntry> _states = new();
        private readonly List<StateEntry> _stateScratch = new();
        private string[] _layerOptions; // ["Any Layer", layer 0 name, ...]
        private string[] _tagOptions;

        private int _previewBindingIndex = -1;
        private AnimationClip _previewClip;
        private GameObject _previewTarget;

        private void OnEnable()
        {
            _dispatcher = serializedObject.FindProperty("dispatcher");
            _bindings = serializedObject.FindProperty("bindings");
            RebuildControllerCache();
        }

        private void OnDisable() => StopPreview();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_dispatcher);
            if (EditorGUI.EndChangeCheck())
            {
                StopPreview();
                serializedObject.ApplyModifiedProperties();
                RebuildControllerCache();
                serializedObject.Update();
            }

            if (_dispatcher.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign an AnimationEventDispatcher to receive animation events.", MessageType.Warning);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Event Bindings", EditorStyles.boldLabel);

            for (int i = 0; i < _bindings.arraySize; i++)
            {
                if (DrawBinding(i)) break; // element removed — bail this repaint
            }

            if (GUILayout.Button("Add Binding"))
            {
                int index = _bindings.arraySize;
                _bindings.InsertArrayElementAtIndex(index);
                _bindings.GetArrayElementAtIndex(index)
                    .FindPropertyRelative("layerIndex").intValue = AnimationEventScope.AnyLayer;
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <returns>true when the element was removed (array indices invalidated).</returns>
        private bool DrawBinding(int index)
        {
            var element = _bindings.GetArrayElementAtIndex(index);
            var scopeKind = element.FindPropertyRelative("scopeKind");
            var stateHash = element.FindPropertyRelative("stateHash");
            var stateName = element.FindPropertyRelative("stateName");
            var tagName = element.FindPropertyRelative("tagName");
            var layerIndex = element.FindPropertyRelative("layerIndex");
            var eventType = element.FindPropertyRelative("eventType");
            var normalizedTime = element.FindPropertyRelative("normalizedTime");
            var response = element.FindPropertyRelative("response");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(scopeKind, new GUIContent("Scope"));
            DrawLayerPopup(layerIndex);

            var kind = (AnimationEventScopeKind)scopeKind.enumValueIndex;
            if (kind == AnimationEventScopeKind.State)
                DrawStatePopup(stateHash, stateName, layerIndex.intValue);
            else if (kind == AnimationEventScopeKind.Tag)
                DrawTagPopup(tagName);

            EditorGUILayout.PropertyField(eventType);

            bool previewable = kind == AnimationEventScopeKind.State
                               && (AnimationEventType)eventType.enumValueIndex == AnimationEventType.OnTime;

            if (_previewBindingIndex == index && (!previewable || _previewTarget == null || _previewClip == null))
                StopPreview();

            if ((AnimationEventType)eventType.enumValueIndex == AnimationEventType.OnTime)
                DrawTimeRow(index, previewable, stateHash, stateName, normalizedTime);

            EditorGUILayout.PropertyField(response);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool removed = GUILayout.Button("Remove", GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);

            if (removed)
            {
                if (_previewBindingIndex == index) StopPreview();
                else if (_previewBindingIndex > index) _previewBindingIndex--;
                _bindings.DeleteArrayElementAtIndex(index);
            }

            return removed;
        }

        private void DrawLayerPopup(SerializedProperty layerIndex)
        {
            if (_layerOptions == null || _layerOptions.Length <= 1)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup("Layer", 0, new[] { "Any Layer" });
                EditorGUI.EndDisabledGroup();
                return;
            }

            int selected = Mathf.Clamp(layerIndex.intValue + 1, 0, _layerOptions.Length - 1);

            EditorGUI.BeginChangeCheck();
            int newSelected = EditorGUILayout.Popup("Layer", selected, _layerOptions);
            if (EditorGUI.EndChangeCheck())
                layerIndex.intValue = newSelected - 1; // index 0 = Any Layer = -1
        }

        private void DrawStatePopup(SerializedProperty stateHash, SerializedProperty stateName, int layerFilter)
        {
            _stateScratch.Clear();
            if (layerFilter == AnimationEventScope.AnyLayer)
            {
                // Dedupe by StateHash — same short name appears on every synced layer;
                // showing "Base Layer/Idle" and "UpperBody/Idle" is pure noise in Any Layer mode.
                foreach (var entry in _states)
                {
                    bool duplicate = false;
                    for (int j = 0; j < _stateScratch.Count; j++)
                    {
                        if (_stateScratch[j].StateHash == entry.StateHash) { duplicate = true; break; }
                    }
                    if (!duplicate)
                        _stateScratch.Add(entry);
                }
            }
            else
            {
                foreach (var entry in _states)
                {
                    if (entry.LayerIndex == layerFilter)
                        _stateScratch.Add(entry);
                }
            }

            if (_stateScratch.Count == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup("State", 0, new[] { "No states found" });
                EditorGUI.EndDisabledGroup();
                return;
            }

            var labels = new string[_stateScratch.Count];
            int selected = -1;
            for (int i = 0; i < _stateScratch.Count; i++)
            {
                // Any Layer: bare name (deduped list needs no layer prefix).
                // Specific layer: DisplayPath disambiguates sub-state-machine nesting.
                labels[i] = layerFilter == AnimationEventScope.AnyLayer
                    ? _stateScratch[i].StateName
                    : _stateScratch[i].DisplayPath;
                if (selected == -1 && _stateScratch[i].StateHash == stateHash.intValue)
                    selected = i;
            }

            if (selected == -1 && stateHash.intValue != 0)
            {
                EditorGUILayout.HelpBox(
                    $"State '{stateName.stringValue}' no longer exists in the controller (renamed, removed, or different layer). Re-select a state.",
                    MessageType.Warning);
            }

            EditorGUI.BeginChangeCheck();
            int newSelected = EditorGUILayout.Popup("State", Mathf.Max(selected, 0), labels);
            if (EditorGUI.EndChangeCheck() || selected == -1 && stateHash.intValue == 0)
            {
                stateHash.intValue = _stateScratch[newSelected].StateHash;
                stateName.stringValue = _stateScratch[newSelected].StateName;
            }
        }

        private void DrawTagPopup(SerializedProperty tagName)
        {
            if (_tagOptions == null || _tagOptions.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "No tags found in the controller. Select a state in the Animator window and set its Tag field, then reselect this object.",
                    MessageType.Info);
                return;
            }

            int selected = -1;
            for (int i = 0; i < _tagOptions.Length; i++)
            {
                if (_tagOptions[i] == tagName.stringValue) { selected = i; break; }
            }

            if (selected == -1 && !string.IsNullOrEmpty(tagName.stringValue))
            {
                EditorGUILayout.HelpBox(
                    $"Tag '{tagName.stringValue}' no longer exists on any state in the controller. Re-select a tag.",
                    MessageType.Warning);
            }

            EditorGUI.BeginChangeCheck();
            int newSelected = EditorGUILayout.Popup("Tag", Mathf.Max(selected, 0), _tagOptions);
            if (EditorGUI.EndChangeCheck() || selected == -1 && string.IsNullOrEmpty(tagName.stringValue))
                tagName.stringValue = _tagOptions[newSelected];
        }

        private void DrawTimeRow(int bindingIndex, bool previewable, SerializedProperty stateHash,
            SerializedProperty stateName, SerializedProperty normalizedTime)
        {
            bool isPreviewing = _previewBindingIndex == bindingIndex;

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Slider(normalizedTime, 0.001f, 1f, new GUIContent("Normalized Time")); // min matches binding's Range guard (threshold 0 never fires)
            bool sliderChanged = EditorGUI.EndChangeCheck();

            bool toggleClicked = false;
            if (previewable)
            {
                var previewIcon = EditorGUIUtility.IconContent(isPreviewing ? "preAudioAutoPlayOff" : "preAudioPlayOff");
                toggleClicked = GUILayout.Button(previewIcon, EditorStyles.iconButton, GUILayout.Width(24));
            }
            EditorGUILayout.EndHorizontal();

            if (toggleClicked)
            {
                if (isPreviewing)
                {
                    StopPreview();
                }
                else
                {
                    StopPreview();
                    StartPreview(bindingIndex, stateHash.intValue, normalizedTime.floatValue);
                }
            }

            if (_previewBindingIndex == bindingIndex && sliderChanged)
                SamplePreview(normalizedTime.floatValue);

            if (_previewBindingIndex == bindingIndex)
            {
                EditorGUILayout.HelpBox(
                    $"Previewing: {stateName.stringValue} @ {normalizedTime.floatValue:F2}",
                    MessageType.Info);
            }
        }

        private void StartPreview(int bindingIndex, int stateHash, float normalizedTime)
        {
            AnimationClip clip = null;
            foreach (var entry in _states)
            {
                if (entry.StateHash == stateHash && entry.Clip != null) { clip = entry.Clip; break; }
            }

            if (clip == null)
            {
                Debug.LogWarning("<color=#FF5F5D>[AnimationEvents]</color> Cannot preview: no AnimationClip found for this state (BlendTrees are not supported).");
                return;
            }

            var dispatcher = _dispatcher.objectReferenceValue as AnimationEventDispatcher;
            _previewTarget = dispatcher != null ? dispatcher.gameObject : null;
            if (_previewTarget == null) return;

            _previewBindingIndex = bindingIndex;
            _previewClip = clip;
            AnimationMode.StartAnimationMode();
            SamplePreview(normalizedTime);
        }

        private void StopPreview()
        {
            if (_previewBindingIndex < 0) return;

            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();

            _previewBindingIndex = -1;
            _previewClip = null;
            _previewTarget = null;
        }

        private void SamplePreview(float normalizedTime)
        {
            if (_previewClip == null || _previewTarget == null) return;
            if (!AnimationMode.InAnimationMode()) return;

            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(_previewTarget, _previewClip, normalizedTime * _previewClip.length);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }

        private void RebuildControllerCache()
        {
            _states.Clear();
            _layerOptions = null;
            _tagOptions = null;

            var dispatcher = _dispatcher.objectReferenceValue as AnimationEventDispatcher;
            if (dispatcher == null) return;

            var animator = dispatcher.GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null) return;

            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) return;

            var layerNames = new List<string> { "Any Layer" };
            var tags = new List<string>();

            for (int layerIndex = 0; layerIndex < controller.layers.Length; layerIndex++)
            {
                var layer = controller.layers[layerIndex];
                layerNames.Add(layer.name);

                // Synced layers have no state machine of their own — they borrow the
                // source layer's states (events still fire with this layer's index).
                var stateMachine = layer.syncedLayerIndex >= 0
                    ? controller.layers[layer.syncedLayerIndex].stateMachine
                    : layer.stateMachine;
                if (stateMachine == null) continue;

                CollectStates(stateMachine, layer.name, layerIndex, tags);
            }

            _layerOptions = layerNames.ToArray();
            _tagOptions = tags.ToArray();
        }

        private void CollectStates(AnimatorStateMachine stateMachine, string layerName, int layerIndex, List<string> tags)
        {
            foreach (var childState in stateMachine.states)
            {
                var state = childState.state;
                _states.Add(new StateEntry
                {
                    DisplayPath = $"{layerName}/{state.name}",
                    StateName = state.name,
                    StateHash = Animator.StringToHash(state.name),
                    LayerIndex = layerIndex,
                    Clip = state.motion as AnimationClip,
                });

                if (!string.IsNullOrEmpty(state.tag) && !tags.Contains(state.tag))
                    tags.Add(state.tag);
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                CollectStates(childMachine.stateMachine, layerName, layerIndex, tags);
            }
        }
    }
}
