using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AKD.AnimationEvents.Editor
{
    [CustomEditor(typeof(AnimationEventBehaviour))]
    public class AnimationEventBehaviourEditor : UnityEditor.Editor
    {
        private SerializedProperty _events;
        private int _previewIndex = -1;
        private AnimationClip _previewClip;
        private GameObject _previewTarget;

        private void OnEnable()
        {
            if (target == null) return;
            _events = serializedObject.FindProperty("_events");
        }

        private void OnDisable()
        {
            StopPreview();
        }

        public override void OnInspectorGUI()
        {
            if (_events == null) return;
            serializedObject.Update();

            EditorGUILayout.LabelField("Animation Events", EditorStyles.boldLabel);

            for (int i = 0; i < _events.arraySize; i++)
            {
                var element = _events.GetArrayElementAtIndex(i);
                var eventName = element.FindPropertyRelative("eventName");
                var eventType = element.FindPropertyRelative("eventType");
                var normalizedTime = element.FindPropertyRelative("normalizedTime");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(eventName, new GUIContent("Event Name"));
                EditorGUILayout.PropertyField(eventType, new GUIContent("Event Type"));

                if (_previewIndex == i && (AnimationEventType)eventType.enumValueIndex != AnimationEventType.OnTime)
                    StopPreview();

                if ((AnimationEventType)eventType.enumValueIndex == AnimationEventType.OnTime)
                {
                    bool isPreviewing = _previewIndex == i;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Slider(normalizedTime, 0f, 1f, new GUIContent("Normalized Time"));
                    bool sliderChanged = EditorGUI.EndChangeCheck();

                    var previewIcon = EditorGUIUtility.IconContent(
                        isPreviewing ? "preAudioAutoPlayOff" : "preAudioPlayOff");
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
                            TryStartPreview(i, normalizedTime.floatValue);
                        }
                    }

                    if (isPreviewing && sliderChanged)
                        SamplePreview(normalizedTime.floatValue);

                    if (isPreviewing)
                    {
                        EditorGUILayout.HelpBox(
                            $"Previewing @ {normalizedTime.floatValue:F2}",
                            MessageType.Info);
                    }
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    if (_previewIndex == i)
                        StopPreview();
                    else if (_previewIndex > i)
                        _previewIndex--;

                    _events.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("Add Event"))
            {
                _events.InsertArrayElementAtIndex(_events.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void TryStartPreview(int index, float normalizedTime)
        {
            var behaviour = target as AnimationEventBehaviour;
            if (behaviour == null) return;

            var controller = GetOwningController(behaviour);
            if (controller == null) return;

            var clip = FindClipForBehaviour(controller, behaviour);
            if (clip == null)
            {
                Debug.LogWarning(
                    "<color=#FF5F5D>[AnimationEvents]</color> Cannot preview: no AnimationClip found for this state (BlendTrees are not supported).");
                return;
            }

            var activeObject = Selection.activeGameObject;
            if (activeObject != null
                && activeObject.TryGetComponent<Animator>(out var selectedAnimator)
                && selectedAnimator.runtimeAnimatorController == controller)
            {
                _previewTarget = activeObject;
            }
            else
            {
                var animators = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
                foreach (var animator in animators)
                {
                    if (animator.runtimeAnimatorController == controller)
                    {
                        _previewTarget = animator.gameObject;
                        break;
                    }
                }
            }

            if (_previewTarget == null)
            {
                Debug.LogWarning(
                    "<color=#FF5F5D>[AnimationEvents]</color> Cannot preview: no Animator found in scene using this controller. Select a GameObject with the matching Animator.");
                return;
            }

            _previewIndex = index;
            _previewClip = clip;
            AnimationMode.StartAnimationMode();
            SamplePreview(normalizedTime);
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

        private void StopPreview()
        {
            if (_previewIndex < 0) return;

            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();

            _previewIndex = -1;
            _previewClip = null;
            _previewTarget = null;
        }

        private static AnimatorController GetOwningController(AnimationEventBehaviour behaviour)
        {
            var assetPath = AssetDatabase.GetAssetPath(behaviour);
            if (string.IsNullOrEmpty(assetPath)) return null;
            return AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
        }

        private static AnimationClip FindClipForBehaviour(AnimatorController controller, AnimationEventBehaviour behaviour)
        {
            foreach (var layer in controller.layers)
            {
                var clip = FindClipInStateMachine(layer.stateMachine, behaviour);
                if (clip != null) return clip;
            }

            return null;
        }

        private static AnimationClip FindClipInStateMachine(AnimatorStateMachine stateMachine, AnimationEventBehaviour behaviour)
        {
            foreach (var childState in stateMachine.states)
            {
                foreach (var b in childState.state.behaviours)
                {
                    if (b == behaviour)
                        return childState.state.motion as AnimationClip;
                }
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                var clip = FindClipInStateMachine(childMachine.stateMachine, behaviour);
                if (clip != null) return clip;
            }

            return null;
        }
    }
}
