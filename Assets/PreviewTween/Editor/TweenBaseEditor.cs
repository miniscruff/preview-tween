namespace PreviewTween
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public enum PreviewMode
    {
        None,
        Playing,
        Paused
    }

    /// <summary>
    /// Custom editor for all tween bases that even includes children properties so you dont have to customize it yourself
    /// for each new tween you create.
    /// </summary>
    [CustomEditor(typeof(TweenBase), true)]
    public sealed class TweenBaseEditor : Editor
    {
        private readonly List<SerializedProperty> _additionalProperties = new List<SerializedProperty>();

        private SerializedProperty _delayProperty;
        private SerializedProperty _durationProperty;
        private SerializedProperty _playModeProperty;
        private SerializedProperty _wrapModeProperty;
        private SerializedProperty _easingModeProperty;
        private SerializedProperty _customCurveProperty;
        private SerializedProperty _onCompleteProperty;

        private Texture2D _easingIcon;

        private TweenBase _tween;
        private PreviewMode _previewMode;

        private string[] _easingNames;

        private bool _cachedIsProSkin;
        private GUIStyle _leftStyle;
        private GUIStyle _middleStyle;
        private GUIStyle _rightStyle;
        private GUIStyle _thumbStyle;

        private readonly Color _barColor = new Color(0.26f, 0.58f, 1f);
        private readonly Color _backgroundColor = new Color(0.15f, 0.15f, 0.15f);

        private Texture2D _recordStartContent;
        private Texture2D _rewindContent;
        private Texture2D _playContent;
        private Texture2D _pauseContent;
        private Texture2D _forwardContent;
        private Texture2D _recordEndContent;

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
                        _additionalProperties.Add(serializedObject.FindProperty(iterator.name));
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
            if (_tween == null)
            {
                _tween = (TweenBase)target;
            }

            LoadStyles();
            LoadTextures();
            LoadEasingNames();

            serializedObject.Update();

            // this draws any inherited values automatically on top
            DrawAdditionalProperties();
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
            _leftStyle = GUI.skin.FindStyle("ButtonLeft");
            _middleStyle = GUI.skin.FindStyle("ButtonMid");
            _rightStyle = GUI.skin.FindStyle("ButtonRight");
            _thumbStyle = GUI.skin.FindStyle("MeTransPlayhead");
        }

        private void LoadTextures()
        {
            if (_playContent != null)
            {
                return;
            }

            string iconFolderPath = EditorHelper.GetProjectDirectory("/Editor/Graphics/Icons/");

            _recordStartContent = AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "RecordStart.png");
            _rewindContent = AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "Rewind.png");
            _playContent = AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "Play.png");
            _pauseContent = AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "Pause.png");
            _forwardContent = AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "Forward.png");
            _recordEndContent = AssetDatabase.LoadAssetAtPath<Texture2D>(iconFolderPath + "RecordEnd.png");
        }

        private void LoadEasingNames()
        {
            if (_easingNames != null)
            {
                return;
            }

            _easingNames = Enum.GetNames(typeof(EasingMode));
            for (int i = 0; i < _easingNames.Length; i++)
            {
                _easingNames[i] = ObjectNames.NicifyVariableName(_easingNames[i]);
                // by replacing our In or Outs with /In and /Out we create a simple 2 level popup
                // which is much easier to find what you are looking for
                if (_easingNames[i].Contains(" In"))
                {
                    _easingNames[i] = _easingNames[i].Replace(" In", " / In");
                }
                else if (_easingNames[i].Contains(" Out"))
                {
                    _easingNames[i] = _easingNames[i].Replace(" Out", " / Out");
                }
            }
        }

        private void DrawAdditionalProperties()
        {
            foreach (SerializedProperty custom in _additionalProperties)
            {
                EditorGUILayout.PropertyField(custom);
            }
            EditorGUILayout.Separator();
        }

        private void DrawPreview()
        {
            // draw our progress bar
            EditorGUI.BeginDisabledGroup(_previewMode != PreviewMode.None);
            Rect progressRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(40));
            float newValue = EditorHelper.PreviewProgress(progressRect, _tween.progress, _barColor, _backgroundColor, _thumbStyle);
            if (!Mathf.Approximately(newValue, _tween.progress))
            {
                _tween.progress = newValue;
                _tween.Apply();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Separator();

            // Draw our controls view inside a centered horizontal layout
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawRecordStartButton();
            DrawRewindButton();
            DrawPlayButton();
            DrawPauseButton();
            DrawForwardButton();
            DrawRecordEndButton();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRecordStartButton()
        {
            if (DrawRecordModeToggle(0f, _recordStartContent, _leftStyle))
            {
                Undo.RecordObject(target, "Recording Start Value");
                _tween.RecordStart();
            }
        }

        private void DrawRecordEndButton()
        {
            if (DrawRecordModeToggle(1f, _recordEndContent, _rightStyle))
            {
                Undo.RecordObject(target, "Recording End Value");
                _tween.RecordEnd();
            }
        }

        private void DrawRewindButton()
        {
            bool shouldBeActive = _tween.progress > 0 && _previewMode == PreviewMode.None;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            if (GUILayout.Button(_rewindContent, _middleStyle))
            {
                _tween.progress = 0f;
                _tween.direction = 1;
                _tween.Apply();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawForwardButton()
        {
            bool shouldBeActive = _tween.progress < 1 && _previewMode == PreviewMode.None;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            if (GUILayout.Button(_forwardContent, _middleStyle))
            {
                _tween.progress = 1f;
                _tween.direction = -1;
                _tween.Apply();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawPlayButton()
        {
            bool before = _previewMode == PreviewMode.Playing || _previewMode == PreviewMode.Paused;
            bool after = GUILayout.Toggle(before, _playContent, _middleStyle);
            if (before != after)
            {
                _previewMode = after ? PreviewMode.Playing : PreviewMode.None;

                // reset our progress only if we are at the end in a wrap once tween
                if (_tween.wrapMode == WrapMode.Once && _tween.progress >= 1f)
                {
                    _tween.progress = 0f;
                }

                _tween.direction = 1;
                _tween.Apply();

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
            bool shouldBeActive = _previewMode == PreviewMode.Playing || _previewMode == PreviewMode.Paused;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            bool before = _previewMode == PreviewMode.Paused;
            bool after = GUILayout.Toggle(before, _pauseContent, _middleStyle);
            if (before != after)
            {
                _previewMode = after ? PreviewMode.Paused : PreviewMode.Playing;
            }
            EditorGUI.EndDisabledGroup();
        }

        private void UpdatePreview()
        {
            // if we are paused we dont want to do anything
            if (_previewMode != PreviewMode.Playing)
            {
                return;
            }

            // keep our delta time to 30fps as unity likes to have a massive delay for our first frame
            float dt = Mathf.Min(Time.deltaTime, 0.033f);
            _tween.Tick(dt);

            if (_tween.progress >= 1f && _tween.wrapMode == WrapMode.Once)
            {
                _previewMode = PreviewMode.None;
                EditorApplication.update -= UpdatePreview;
            }
        }

        private bool DrawRecordModeToggle(float currentProgress, Texture texture, GUIStyle buttonStyle)
        {
            bool shouldBeActive = Mathf.Approximately(_tween.progress, currentProgress) && _previewMode == PreviewMode.None;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            bool result = GUILayout.Button(texture, buttonStyle);
            EditorGUI.EndDisabledGroup();
            return result;
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
            if (_durationProperty.floatValue < TweenBase.minimum_duration)
            {
                _durationProperty.floatValue = TweenBase.minimum_duration;
            }

            EditorGUILayout.PropertyField(_playModeProperty);
            EditorGUILayout.PropertyField(_wrapModeProperty);

            int newEasingIndex = EditorGUILayout.Popup(_easingModeProperty.displayName, _easingModeProperty.enumValueIndex, _easingNames);
            EasingMode easingMode = (EasingMode)newEasingIndex;

            if (newEasingIndex != _easingModeProperty.enumValueIndex || _easingIcon == null && easingMode != EasingMode.CustomCurve)
            {
                _easingModeProperty.enumValueIndex = newEasingIndex;
                if (easingMode != EasingMode.CustomCurve)
                {
                    string enumString = ((EasingMode)newEasingIndex).ToString();
                    string easingIconPath = EditorHelper.GetProjectDirectory("/Editor/Graphics/Easings/") + enumString + ".png";
                    _easingIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(easingIconPath);
                }
                else
                {
                    _easingIcon = null;
                }
            }

            if (easingMode == EasingMode.CustomCurve)
            {
                EditorGUILayout.PropertyField(_customCurveProperty, GUILayout.Height(100));
            }
            else
            {
                Rect easingIconRect = GUILayoutUtility.GetRect(_easingIcon.width, _easingIcon.height, GUIStyle.none);
                GUI.DrawTexture(easingIconRect, _easingIcon, ScaleMode.ScaleToFit);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_onCompleteProperty);
        }
    }
}