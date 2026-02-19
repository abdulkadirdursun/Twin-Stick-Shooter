using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AKD.AnimationEvents.Editor
{
    [CustomEditor(typeof(AnimationEventListener))]
    public class AnimationEventListenerEditor : UnityEditor.Editor
    {
        private SerializedProperty _dispatcher;
        private SerializedProperty _bindings;

        private string[] _stateNames;
        private int[] _stateHashes;

        private int _previewBindingIndex = -1;
        private AnimationClip _previewClip;
        private GameObject _previewTarget;

        private void OnEnable()
        {
            _dispatcher = serializedObject.FindProperty("dispatcher");
            _bindings = serializedObject.FindProperty("bindings");

            RebuildStateList();
        }

        private void OnDisable()
        {
            StopPreview();
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_dispatcher);
            if (EditorGUI.EndChangeCheck())
            {
                StopPreview();
                serializedObject.ApplyModifiedProperties();
                RebuildStateList();
                serializedObject.Update();
            }

            if (_dispatcher.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign an AnimationEventDispatcher to receive animation events.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Event Bindings", EditorStyles.boldLabel);

            for (int i = 0; i < _bindings.arraySize; i++)
            {
                var element = _bindings.GetArrayElementAtIndex(i);
                var stateHash = element.FindPropertyRelative("stateHash");
                var stateName = element.FindPropertyRelative("stateName");
                var eventType = element.FindPropertyRelative("eventType");
                var normalizedTime = element.FindPropertyRelative("normalizedTime");
                var response = element.FindPropertyRelative("response");

                string label = string.IsNullOrEmpty(stateName.stringValue)
                    ? $"Binding {i}"
                    : stateName.stringValue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                DrawStateDropdown(label, stateHash, stateName);

                EditorGUILayout.PropertyField(eventType);

                if (_previewBindingIndex == i && (_previewTarget == null || _previewClip == null))
                {
                    StopPreview();
                }

                if (_previewBindingIndex == i && (AnimationEventType)eventType.enumValueIndex != AnimationEventType.OnTime)
                {
                    StopPreview();
                }

                if ((AnimationEventType)eventType.enumValueIndex == AnimationEventType.OnTime)
                {
                    bool isPreviewing = _previewBindingIndex == i;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Slider(normalizedTime, 0f, 1f, new GUIContent("Normalized Time"));
                    bool sliderChanged = EditorGUI.EndChangeCheck();

                    var previewIcon = EditorGUIUtility.IconContent(isPreviewing ? "preAudioAutoPlayOff" : "preAudioPlayOff");
                    bool toggleClicked = GUILayout.Button(previewIcon, EditorStyles.iconButton, GUILayout.Width(24));
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

                            var clip = FindClipForState(stateHash.intValue);
                            if (clip == null)
                            {
                                Debug.LogWarning("<color=#FF5F5D>[AnimationEvents]</color> Cannot preview: no AnimationClip found for this state (BlendTrees are not supported).");
                            }
                            else
                            {
                                var dispatcher = _dispatcher.objectReferenceValue as AnimationEventDispatcher;
                                _previewTarget = dispatcher != null ? dispatcher.gameObject : null;

                                if (_previewTarget != null)
                                {
                                    _previewBindingIndex = i;
                                    _previewClip = clip;
                                    AnimationMode.StartAnimationMode();
                                    SamplePreview(normalizedTime.floatValue);
                                }
                            }
                        }
                    }

                    if (isPreviewing && sliderChanged)
                    {
                        SamplePreview(normalizedTime.floatValue);
                    }

                    if (isPreviewing)
                    {
                        EditorGUILayout.HelpBox(
                            $"Previewing: {stateName.stringValue} @ {normalizedTime.floatValue:F2}",
                            MessageType.Info);
                    }
                }

                EditorGUILayout.PropertyField(response);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    if (_previewBindingIndex == i)
                        StopPreview();
                    else if (_previewBindingIndex > i)
                        _previewBindingIndex--;

                    _bindings.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("Add Binding"))
            {
                _bindings.InsertArrayElementAtIndex(_bindings.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStateDropdown(string label, SerializedProperty stateHash, SerializedProperty stateName)
        {
            if (_stateNames == null || _stateNames.Length == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup("State", 0, new[] { "No states found" });
                EditorGUI.EndDisabledGroup();
                return;
            }

            int currentHash = stateHash.intValue;
            int selectedIndex = -1;

            for (int i = 0; i < _stateHashes.Length; i++)
            {
                if (_stateHashes[i] == currentHash)
                {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex == -1) selectedIndex = 0;

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup("State", selectedIndex, _stateNames);
            if (EditorGUI.EndChangeCheck())
            {
                stateHash.intValue = _stateHashes[newIndex];
                stateName.stringValue = _stateNames[newIndex];
            }
        }

        private void RebuildStateList()
        {
            _stateNames = null;
            _stateHashes = null;

            var dispatcher = _dispatcher.objectReferenceValue as AnimationEventDispatcher;
            if (dispatcher == null) return;

            var animator = dispatcher.GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null) return;

            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) return;

            var names = new List<string>();
            var hashes = new List<int>();

            foreach (var layer in controller.layers)
            {
                CollectStates(layer.stateMachine, names, hashes);
            }

            _stateNames = names.ToArray();
            _stateHashes = hashes.ToArray();
        }

        private static void CollectStates(AnimatorStateMachine stateMachine, List<string> names, List<int> hashes)
        {
            foreach (var childState in stateMachine.states)
            {
                string name = childState.state.name;
                names.Add(name);
                hashes.Add(Animator.StringToHash(name));
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                CollectStates(childMachine.stateMachine, names, hashes);
            }
        }

        private AnimationClip FindClipForState(int stateHash)
        {
            var dispatcher = _dispatcher.objectReferenceValue as AnimationEventDispatcher;
            if (dispatcher == null) return null;

            var animator = dispatcher.GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null) return null;

            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) return null;

            foreach (var layer in controller.layers)
            {
                var clip = FindClipInStateMachine(layer.stateMachine, stateHash);
                if (clip != null) return clip;
            }

            return null;
        }

        private static AnimationClip FindClipInStateMachine(AnimatorStateMachine stateMachine, int stateHash)
        {
            foreach (var childState in stateMachine.states)
            {
                if (Animator.StringToHash(childState.state.name) == stateHash)
                    return childState.state.motion as AnimationClip;
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                var clip = FindClipInStateMachine(childMachine.stateMachine, stateHash);
                if (clip != null) return clip;
            }

            return null;
        }
    }
}
