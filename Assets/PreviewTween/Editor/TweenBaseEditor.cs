namespace PreviewTween
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TweenBase), true)]
    public sealed class TweenBaseEditor : Editor
    {
        private readonly List<SerializedProperty> _customProperties = new List<SerializedProperty>();

        private SerializedProperty _delayProperty;
        private SerializedProperty _durationProperty;
        private SerializedProperty _playModeProperty;
        private SerializedProperty _wrapModeProperty;
        private SerializedProperty _easingModeProperty;
        private SerializedProperty _customCurveProperty;
        private SerializedProperty _onCompleteProperty;

        private TweenBase _tween;
        private bool _isPlaying;

        private void OnEnable()
        {
            _tween = (TweenBase)target;

            _delayProperty = serializedObject.FindProperty("_delay");
            _durationProperty = serializedObject.FindProperty("_duration");
            _playModeProperty = serializedObject.FindProperty("_playMode");
            _wrapModeProperty = serializedObject.FindProperty("_wrapMode");
            _easingModeProperty = serializedObject.FindProperty("_easingMode");
            _customCurveProperty = serializedObject.FindProperty("_customCurve");
            _onCompleteProperty = serializedObject.FindProperty("_onComplete");

            SerializedProperty iterator = serializedObject.GetIterator();
            // we need to go into our first item at least
            iterator.NextVisible(true);

            while(iterator.NextVisible(false))
            {
                switch (iterator.name)
                {
                    case "_delay":
                    case "_duration":
                    case "_playMode":
                    case "_wrapMode":
                    case "_easingMode":
                    case "_customCurve":
                    case "_onComplete":
                        break;
                    // all the values that our custom tween provides will go in here and be visible at the top of our editor
                    default:
                        _customProperties.Add(serializedObject.FindProperty(iterator.name));
                        break;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            foreach (SerializedProperty custom in _customProperties)
            {
                EditorGUILayout.PropertyField(custom);
            }
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(_isPlaying ? "Stop" : "Play"))
            {
                _tween.progress = 0f;
                _tween.direction = 1;
                _tween.Apply();

                if (_isPlaying)
                {
                    EditorApplication.update -= UpdatePreview;
                }
                else
                {
                    EditorApplication.update += UpdatePreview;
                }
                _isPlaying = !_isPlaying;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Slider(_tween.progress, 0f, 1f);

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_delayProperty);
            if (_delayProperty.floatValue < 0)
            {
                _delayProperty.floatValue = 0f;
            }

            EditorGUILayout.PropertyField(_durationProperty);
            if (_durationProperty.floatValue < 0.1f)
            {
                _durationProperty.floatValue = 0.1f;
            }

            EditorGUILayout.PropertyField(_playModeProperty);
            EditorGUILayout.PropertyField(_wrapModeProperty);
            EditorGUILayout.PropertyField(_easingModeProperty);

            EasingMode easingMode = (EasingMode)_easingModeProperty.enumValueIndex;
            if (easingMode == EasingMode.CustomCurve)
            {
                EditorGUILayout.PropertyField(_customCurveProperty);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_onCompleteProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdatePreview()
        {
            // keep our delta time to 30fps as unity likes to have a massive delay for our first frame
            float dt = Mathf.Min(Time.deltaTime, 0.033f);
            _tween.Tick(dt);

            if (_tween.progress >= 1f && _tween.wrapMode == WrapMode.Once)
            {
                _isPlaying = false;
                _tween.progress = 0f;
                EditorApplication.update -= UpdatePreview;
            }
        }
    }
}