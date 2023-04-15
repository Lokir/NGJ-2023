// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(MoveSensor))]
    public class MoveSensorInspector : Editor
    {
        SerializedProperty IsConnectedChanged;
        SerializedProperty SpeedChanged;

        void OnEnable()
        {
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
            SpeedChanged = serializedObject.FindProperty("SpeedChanged");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Label("Status", EditorStyles.boldLabel);
            MoveSensor move = serializedObject.targetObject as MoveSensor;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", move.IsConnected);
            EditorGUILayout.IntField("Speed", move.Speed);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            EditorGUILayout.PropertyField(SpeedChanged);

            serializedObject.ApplyModifiedProperties();
        }
    }
}