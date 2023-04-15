// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(Motor))]
    public class MotorInspector : Editor
    {
        SerializedProperty IsConnectedChanged;

        void OnEnable()
        {
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Motor motor = serializedObject.targetObject as Motor;

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", motor.IsConnected);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);

            serializedObject.ApplyModifiedProperties();
        }

        private Rect MakeBg(Color color)
        {
            Rect fieldRect = EditorGUILayout.GetControlRect(hasLabel: true, height: EditorGUIUtility.singleLineHeight, style: EditorStyles.colorField);
            Rect highlightRect = new Rect(
                    x: fieldRect.x + EditorGUIUtility.labelWidth,
                    y: fieldRect.y - 2,
                    width: fieldRect.width + 2 - EditorGUIUtility.labelWidth,
                    height: fieldRect.height + 2 * 2);
            EditorGUI.DrawRect(highlightRect, color);
            return fieldRect;
        }
    }
}