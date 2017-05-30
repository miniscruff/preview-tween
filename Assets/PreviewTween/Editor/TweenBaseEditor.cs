namespace PreviewTween
{
    using System.Collections.Generic;
    using UnityEditor;

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

        private void OnEnable()
        {
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

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_delayProperty);
            EditorGUILayout.PropertyField(_durationProperty);
            EditorGUILayout.PropertyField(_playModeProperty);
            EditorGUILayout.PropertyField(_wrapModeProperty);
            EditorGUILayout.PropertyField(_easingModeProperty);
            EditorGUILayout.PropertyField(_customCurveProperty);
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_onCompleteProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}