

using FileRenamer.Styles;
using System;
using UnityEditor;
using UnityEngine;

namespace FileRenamer
{
    public class ElementsGUI
    {
        #region Settings 

        private const float DefaultToggleSpacing = 5F;

        #endregion


        #region TextField 

        public static void DisplayTextFieldShort(string label, string text, Action<string> setter)
        {
            DisplayTextField(label, text, setter, GUILayout.MaxWidth(FileRenamerStyleGUI.ShortTextFieldMaxWidth));
        }

        public static void DisplayTextField(string label, string text, Action<string> setter, params GUILayoutOption[] options)
        {
            string input = EditorGUILayout.TextField(label, text, options);
            setter(input);
        }

        #endregion


        #region Toggle 



        public static void DisplayToggle(string label, ref bool relatedParameter)
        {
            DisplayCustomToggle(label, ref relatedParameter, FileRenamerStyleGUI.ToggleLabelLayouts);
        }

        public static void DisplayToggle(string label, Func<bool> getter, Action<bool> setter)
        {
            DisplayCustomToggle(label, getter, setter, FileRenamerStyleGUI.ToggleLabelLayouts);
        }

        public static void DisplayCustomToggle(string label, ref bool relatedParameter, params GUILayoutOption[] labelOptions)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(label, labelOptions);
            relatedParameter = EditorGUILayout.Toggle(relatedParameter);

            GUILayout.Space(DefaultToggleSpacing);
            EditorGUILayout.EndHorizontal();
        }

        public static void DisplayCustomToggle(string label, Func<bool> getter, Action<bool> setter, params GUILayoutOption[] labelOptions)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(label, labelOptions);
            bool newValue = EditorGUILayout.Toggle(getter());
            setter(newValue);

            GUILayout.Space(DefaultToggleSpacing);
            EditorGUILayout.EndHorizontal();
        }

        #endregion


        #region Line

        public static void DrawLine(Color color, float thickness = 2f)
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            EditorGUI.DrawRect(rect, color);
            EditorGUILayout.Space();
        }

        #endregion
    }
}
