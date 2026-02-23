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

        private string[] _layerNames;
        private Dictionary<int, List<string>> _eventNamesByLayer;

        private void OnEnable()
        {
            _dispatcher = serializedObject.FindProperty("_dispatcher");
            _bindings = serializedObject.FindProperty("_bindings");

            RebuildEventData();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_dispatcher);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                RebuildEventData();
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
                var layerIndex = element.FindPropertyRelative("layerIndex");
                var eventName = element.FindPropertyRelative("eventName");
                var response = element.FindPropertyRelative("response");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                DrawLayerDropdown(layerIndex);
                DrawEventNameDropdown(layerIndex.intValue, eventName);

                EditorGUILayout.PropertyField(response);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
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

        private void DrawLayerDropdown(SerializedProperty layerIndex)
        {
            if (_layerNames == null || _layerNames.Length == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup("Layer", 0, new[] { "No layers found" });
                EditorGUI.EndDisabledGroup();
                return;
            }

            int current = Mathf.Clamp(layerIndex.intValue, 0, _layerNames.Length - 1);
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup("Layer", current, _layerNames);
            if (EditorGUI.EndChangeCheck())
            {
                layerIndex.intValue = newIndex;
            }
        }

        private void DrawEventNameDropdown(int layerIdx, SerializedProperty eventName)
        {
            if (_eventNamesByLayer == null
                || !_eventNamesByLayer.TryGetValue(layerIdx, out var names)
                || names.Count == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup("Event", 0, new[] { "No events on this layer" });
                EditorGUI.EndDisabledGroup();
                return;
            }

            int selectedIndex = names.IndexOf(eventName.stringValue);
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
                eventName.stringValue = names[0];
            }

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup("Event", selectedIndex, names.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                eventName.stringValue = names[newIndex];
            }
        }

        private void RebuildEventData()
        {
            _layerNames = null;
            _eventNamesByLayer = null;

            var dispatcher = _dispatcher.objectReferenceValue as AnimationEventDispatcher;
            if (dispatcher == null) return;

            var animator = dispatcher.GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null) return;

            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) return;

            var layers = controller.layers;
            _layerNames = new string[layers.Length];
            _eventNamesByLayer = new Dictionary<int, List<string>>();

            for (int i = 0; i < layers.Length; i++)
            {
                _layerNames[i] = $"{i}: {layers[i].name}";
                var events = new List<string>();
                CollectEventNames(layers[i].stateMachine, events);
                _eventNamesByLayer[i] = events;
            }
        }

        private static void CollectEventNames(AnimatorStateMachine stateMachine, List<string> eventNames)
        {
            foreach (var childState in stateMachine.states)
            {
                foreach (var behaviour in childState.state.behaviours)
                {
                    if (behaviour is AnimationEventBehaviour aeb)
                    {
                        foreach (var entry in aeb.Events)
                        {
                            if (!string.IsNullOrEmpty(entry.eventName) && !eventNames.Contains(entry.eventName))
                                eventNames.Add(entry.eventName);
                        }
                    }
                }
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                CollectEventNames(childMachine.stateMachine, eventNames);
            }
        }
    }
}
