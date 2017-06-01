namespace PreviewTween
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public enum PreviewMode
    {
        None,
        RecordStart,
        RecordEnd,
        Playing,
        Paused
    }

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
        private PreviewMode _mode;

        private bool _cachedIsProSkin;
        private GUIStyle _leftStyle;
        private GUIStyle _middleStyle;
        private GUIStyle _rightStyle;
        private GUIStyle _thumbStyle;

        //private GUIContent _recordStartNormal;
        //private GUIContent _recordStartOn;
        private GUIContent _playNormal;
        private GUIContent _playOn;
        private GUIContent _pauseNormal;
        private GUIContent _pauseOn;
        //private GUIContent _recordStartNormal;
        //private GUIContent _recordStartOn;

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

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            LoadStyles();
            LoadTextures();

            serializedObject.Update();

            DrawCustom();
            DrawPreview();
            DrawSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void LoadStyles()
        {
            // we will need to reload our styles if we change from pro to personal skins
            if (_leftStyle != null && EditorGUIUtility.isProSkin == _cachedIsProSkin)
            {
                return;
            }

            _cachedIsProSkin = EditorGUIUtility.isProSkin;
            _leftStyle = GUI.skin.FindStyle("LargeButtonLeft");
            _middleStyle = GUI.skin.FindStyle("LargeButtonMid");
            _rightStyle = GUI.skin.FindStyle("LargeButtonRight");
            _thumbStyle = GUI.skin.FindStyle("MeTransPlayhead");
        }

        private void LoadTextures()
        {
            if (_playNormal != null)
            {
                return;
            }

            string iconFolderPath = EditorHelper.GetProjectDirectory("/Editor/Graphics/Icons/");

            _playNormal = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "PlayNormal.png"));
            _playOn = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "PlayOn.png"));
            _pauseNormal = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "PauseNormal.png"));
            _pauseOn = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "PauseOn.png"));
        }

        private void DrawCustom()
        {
            foreach (SerializedProperty custom in _customProperties)
            {
                EditorGUILayout.PropertyField(custom);
            }
            EditorGUILayout.Separator();
        }

        private void DrawPreview()
        {
            EditorGUI.BeginDisabledGroup(_mode != PreviewMode.None);
            Rect progressRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(40));
            float newValue = EditorHelper.PreviewProgress(progressRect, _tween.progress, _thumbStyle);
            if (!Mathf.Approximately(newValue, _tween.progress))
            {
                _tween.progress = newValue;
                _tween.Apply();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawRecordStartButton();
            DrawRewindButton();
            DrawPlayButton();
            DrawPauseButton();
            DrawRecordEndButton();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRecordStartButton()
        {
            if (DrawRecordModeToggle(PreviewMode.RecordStart, PreviewMode.RecordEnd, "Rec S", _leftStyle))
            {
                if (_mode == PreviewMode.RecordStart)
                {
                    _tween.progress = 0f;
                    _tween.Apply();
                    EditorApplication.update += UpdateRecordStart;
                    EditorApplication.update -= UpdateRecordEnd;
                }
                else
                {
                    EditorApplication.update -= UpdateRecordStart;
                }
            }
        }

        private void DrawRecordEndButton()
        {
            if (DrawRecordModeToggle(PreviewMode.RecordEnd, PreviewMode.RecordStart, "Rec E", _rightStyle))
            {
                if (_mode == PreviewMode.RecordEnd)
                {
                    _tween.progress = 1f;
                    _tween.Apply();
                    EditorApplication.update += UpdateRecordEnd;
                    EditorApplication.update -= UpdateRecordStart;
                }
                else
                {
                    EditorApplication.update -= UpdateRecordEnd;
                }
            }
        }

        private void UpdateRecordStart()
        {
            _tween.RecordStart();
        }

        private void UpdateRecordEnd()
        {
            _tween.RecordEnd();
        }

        private void DrawRewindButton()
        {
            bool shouldBeActive = _tween.progress > 0 && _mode == PreviewMode.None;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            if (GUILayout.Button("Re", _middleStyle))
            {
                _tween.progress = 0f;
                _tween.direction = 1;
                _tween.Apply();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawPlayButton()
        {
            bool before = _mode == PreviewMode.Playing || _mode == PreviewMode.Paused;
            bool after = GUILayout.Toggle(before, before ? _playOn : _playNormal, _middleStyle);
            if (before != after)
            {
                _mode = after ? PreviewMode.Playing : PreviewMode.None;

                // reset our progress only if we are at the end in a wrap once tween
                if (_tween.wrapMode == WrapMode.Once && _tween.progress >= 1f)
                {
                    _tween.progress = 0f;
                }

                _tween.direction = 1;
                _tween.Apply();

                EditorApplication.update -= UpdateRecordStart;
                EditorApplication.update -= UpdateRecordEnd;

                if (after)
                {
                    EditorApplication.update += UpdatePreview;
                }
                else
                {
                    EditorApplication.update -= UpdatePreview;
                }
            }
        }

        private void DrawPauseButton()
        {
            bool shouldBeActive = _mode == PreviewMode.Playing || _mode == PreviewMode.Paused;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            bool before = _mode == PreviewMode.Paused;
            bool after = GUILayout.Toggle(before, before ? _pauseOn : _pauseNormal, _middleStyle);
            if (before != after)
            {
                _mode = after ? PreviewMode.Paused : PreviewMode.Playing;
            }
            EditorGUI.EndDisabledGroup();
        }

        private void UpdatePreview()
        {
            // if we are paused we dont want to do anything
            if (_mode != PreviewMode.Playing)
            {
                return;
            }

            // keep our delta time to 30fps as unity likes to have a massive delay for our first frame
            float dt = Mathf.Min(Time.deltaTime, 0.033f);
            _tween.Tick(dt);

            if (_tween.progress >= 1f && _tween.wrapMode == WrapMode.Once)
            {
                _mode = PreviewMode.None;
                EditorApplication.update -= UpdatePreview;
            }
        }

        private bool DrawRecordModeToggle(PreviewMode mode, PreviewMode otherRecord, string text, GUIStyle leftStyle, params GUILayoutOption[] options)
        {
            bool shouldBeActive = _mode == PreviewMode.None || _mode == mode || _mode == otherRecord;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            bool before = _mode == mode;
            bool after = GUILayout.Toggle(before, text, leftStyle, options);
            if (before != after)
            {
                _mode = after ? mode : PreviewMode.None;
            }
            EditorGUI.EndDisabledGroup();
            return before != after;
        }

        private void DrawSettings()
        {
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

            EasingMode easingMode = (EasingMode) _easingModeProperty.enumValueIndex;
            if (easingMode == EasingMode.CustomCurve)
            {
                EditorGUILayout.PropertyField(_customCurveProperty);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_onCompleteProperty);
        }
    }
}