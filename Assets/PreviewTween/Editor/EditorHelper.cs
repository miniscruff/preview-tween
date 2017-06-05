namespace PreviewTween
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class EditorHelper
    {
        public static float PreviewProgress(Rect controlRect, float value, Color barColor, Color backgroundColor, GUIStyle thumbStyle)
        {
            Texture2D thumbTexture = thumbStyle.normal.background;

            // give our slider a little bit of a buffer
            Rect sliderRect = new Rect
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
                        Rect backgroundRect = new Rect(sliderRect);
                        backgroundRect.x -= 2f;
                        backgroundRect.width += 4f;
                        backgroundRect.y -= 2f;
                        backgroundRect.height += 4f;

                        EditorGUI.DrawRect(backgroundRect, backgroundColor);

                        Rect drawingRect = new Rect(sliderRect);
                        drawingRect.width *= value;

                        EditorGUI.DrawRect(drawingRect, barColor);
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
                float relativeX = Event.current.mousePosition.x - sliderRect.x;

                // Divide by control width to get a value between 0 and 1
                value = Mathf.Clamp01(relativeX / sliderRect.width);

                // Report that the data in the GUI has changed
                GUI.changed = true;

                // Mark event as 'used' so other controls don't respond to it, and to
                // trigger an automatic repaint.
                Event.current.Use();
            }

            // calculate where our thumb is now
            Rect thumbRect = new Rect
            {
                x = sliderRect.x + value * sliderRect.width - thumbTexture.width / 2f,
                y = controlRect.y,
                width = thumbTexture.width,
                height = thumbTexture.height
            };

            // Draw the thumb texture
            GUI.DrawTexture(thumbRect, thumbTexture);
            return value;
        }

        public static string GetProjectDirectory(string childPath)
        {
            string iconFolderPath = null;
            string[] allDirectories = Directory.GetDirectories("Assets", "PreviewTween", SearchOption.AllDirectories);
            foreach (string dir in allDirectories)
            {
                iconFolderPath = dir + childPath;
                if (Directory.Exists(iconFolderPath))
                {
                    break;
                }
            }

            return iconFolderPath;
        }
    }
}