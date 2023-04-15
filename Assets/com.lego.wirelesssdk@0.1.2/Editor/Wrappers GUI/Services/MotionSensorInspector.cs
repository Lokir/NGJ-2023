// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(MotionSensor))]
    public class MotionSensorInspector : Editor
    {
        SerializedProperty IsConnectedChanged;
        SerializedProperty DistanceChanged;
        SerializedProperty DetectChanged;

        void OnEnable()
        {
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
            DistanceChanged = serializedObject.FindProperty("DistanceChanged");
            DetectChanged = serializedObject.FindProperty("DetectChanged");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MotionSensor motion = serializedObject.targetObject as MotionSensor;

            var newMode = (MotionSensor.MotionSensorMode)EditorGUILayout.EnumPopup("Mode", motion.Mode);
            if (newMode != motion.Mode)
            {
                Undo.RegisterCompleteObjectUndo(motion, "Changed motion sensor mode");
                motion.Mode = newMode;
            }

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", motion.IsConnected);
            if (motion.Mode == MotionSensor.MotionSensorMode.Distance)
            {
                EditorGUILayout.IntField("Distance", motion.Distance);
            }
            if (motion.Mode == MotionSensor.MotionSensorMode.Detect)
            {
                EditorGUILayout.Toggle("Detect", motion.Detect);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            if (motion.Mode == MotionSensor.MotionSensorMode.Distance)
            {
                EditorGUILayout.PropertyField(DistanceChanged);
            }
            if (motion.Mode == MotionSensor.MotionSensorMode.Detect)
            {
                EditorGUILayout.PropertyField(DetectChanged);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}