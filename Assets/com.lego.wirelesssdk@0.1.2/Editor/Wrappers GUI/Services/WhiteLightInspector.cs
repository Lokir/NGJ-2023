// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(WhiteLight))]
    public class WhiteLightInspector : Editor
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

            GUILayout.Label("Status", EditorStyles.boldLabel);
            WhiteLight light = serializedObject.targetObject as WhiteLight;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", light.IsConnected);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);

            serializedObject.ApplyModifiedProperties();
        }
    }
}