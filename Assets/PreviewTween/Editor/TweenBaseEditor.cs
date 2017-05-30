namespace PreviewTween
{
    using System.Collections.Generic;
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

            DrawCustom();
            DrawPreview();
            DrawSettings();

            serializedObject.ApplyModifiedProperties();
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
            GUIStyle leftStyle = GUI.skin.FindStyle("LargeButtonLeft");
            GUIStyle middleStyle = GUI.skin.FindStyle("LargeButtonMid");
            GUIStyle rightStyle = GUI.skin.FindStyle("LargeButtonRight");
            GUIStyle progressBarStyle = GUI.skin.FindStyle("ProgressBarBack");
            GUIStyle thumbStyle = GUI.skin.FindStyle("MeTransPlayhead");

            EditorGUILayout.BeginHorizontal();

            DrawRecordStartButton(leftStyle);
            DrawRewindButton(middleStyle);
            DrawPlayButton(middleStyle);
            DrawPauseButton(middleStyle);
            DrawRecordEndButton(rightStyle);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUI.BeginDisabledGroup(_mode != PreviewMode.None);
            //float newValue = EditorGUILayout.Slider(_tween.progress, 0f, 1f);
            Rect progressRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(40));
            float newValue = PreviewProgress(progressRect, _tween.progress, progressBarStyle, thumbStyle);
            if (!Mathf.Approximately(newValue, _tween.progress))
            {
                _tween.progress = newValue;
                _tween.Apply();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Separator();
        }

        private void DrawRecordStartButton(GUIStyle leftStyle)
        {
            if (DrawRecordModeToggle(PreviewMode.RecordStart, PreviewMode.RecordEnd, "Rec S", leftStyle, GUILayout.Width(60)))
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

        private void DrawRecordEndButton(GUIStyle rightStyle)
        {
            if (DrawRecordModeToggle(PreviewMode.RecordEnd, PreviewMode.RecordStart, "Rec E", rightStyle, GUILayout.Width(60)))
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

        private void DrawRewindButton(GUIStyle middleStyle)
        {
            bool shouldBeActive = _tween.progress > 0 && _mode == PreviewMode.None;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            if (GUILayout.Button("Re", middleStyle, GUILayout.Width(60)))
            {
                _tween.progress = 0f;
                _tween.direction = 1;
                _tween.Apply();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawPlayButton(GUIStyle middleStyle)
        {
            bool before = _mode == PreviewMode.Playing || _mode == PreviewMode.Paused;
            bool after = GUILayout.Toggle(before, "Pl", middleStyle, GUILayout.Width(60));
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

        private void DrawPauseButton(GUIStyle middleStyle)
        {
            bool shouldBeActive = _mode == PreviewMode.Playing || _mode == PreviewMode.Paused;
            EditorGUI.BeginDisabledGroup(!shouldBeActive);

            bool before = _mode == PreviewMode.Paused;
            bool after = GUILayout.Toggle(before, "Pa", middleStyle, GUILayout.Width(60));
            if (before != after)
            {
                _mode = after ? PreviewMode.Paused : PreviewMode.Playing;
            }
            EditorGUI.EndDisabledGroup();
        }

        private static float PreviewProgress(Rect controlRect, float value, GUIStyle barStyle, GUIStyle thumbStyle)
        {
            Texture2D thumbTexture = thumbStyle.normal.background;

            // give our slider a little bit of a buffer
            Rect backgroundRect = new Rect
            {
                x = controlRect.x + thumbTexture.width / 2f,
                y = controlRect.y + thumbTexture.height,
                width = controlRect.width - thumbTexture.width,
                height = controlRect.height - thumbTexture.height
            };

            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.Repaint:
                    {
                        // Draw the texture from the GUIStyle
                        GUI.DrawTexture(backgroundRect, barStyle.normal.background, ScaleMode.StretchToFill);
                        break;
                    }
                case EventType.MouseDown:
                    {
                        // If the click is actually on us...
                        // ...and the click is with the left mouse button (button 0)...
                        if (controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            // ...then capture the mouse by setting the hotControl.
                            GUIUtility.hotControl = controlId;
                        }

                        break;
                    }
                case EventType.MouseUp:
                    {
                        // If we were the hotControl, we aren't any more.
                        if (GUIUtility.hotControl == controlId)
                        {
                            GUIUtility.hotControl = 0;
                        }

                        break;
                    }
            }

            if (Event.current.isMouse && GUIUtility.hotControl == controlId)
            {
                // Get mouse X position relative to left edge of the control
                float relativeX = Event.current.mousePosition.x - backgroundRect.x;

                // Divide by control width to get a value between 0 and 1
                value = Mathf.Clamp01(relativeX / backgroundRect.width);

                // Report that the data in the GUI has changed
                GUI.changed = true;

                // Mark event as 'used' so other controls don't respond to it, and to
                // trigger an automatic repaint.
                Event.current.Use();
            }

            // calculate where our thumb is now
            Rect thumbRect = new Rect
            {
                x = backgroundRect.x + value * backgroundRect.width - thumbTexture.width / 2f,
                y = controlRect.y,
                width = thumbTexture.width,
                height = thumbTexture.height
            };

            // Draw the thumb texture
            GUI.DrawTexture(thumbRect, thumbTexture);

            return value;
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