// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using System;
using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(TachoMotorWithAbsolutePosition))]
    public class TachoMotorWithAbsolutePositionInspector : Editor
    {
        SerializedProperty IsConnectedChanged;
        SerializedProperty PositionChanged;
        SerializedProperty SpeedChanged;
        SerializedProperty PowerChanged;

        void OnEnable()
        {
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
            PositionChanged = serializedObject.FindProperty("PositionChanged");
            SpeedChanged = serializedObject.FindProperty("SpeedChanged");
            PowerChanged = serializedObject.FindProperty("PowerChanged");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TachoMotorWithAbsolutePosition motor = serializedObject.targetObject as TachoMotorWithAbsolutePosition;

            var newMode = (TachoMotorWithAbsolutePosition.TachoMotorMode)EditorGUILayout.EnumPopup("Mode", motor.Mode);
            if (newMode != motor.Mode)
            {
                Undo.RegisterCompleteObjectUndo(motor, "Changed motor mode");
                motor.Mode = newMode;
            }

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", motor.IsConnected);
            if (motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.AbsolutePosition || motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.Position || motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.IntField("Position", motor.Position);
            }
            if (motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.Speed || motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.IntField("Speed", motor.Speed);

            }
            if (motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.Power)
            {
                EditorGUILayout.IntField("Power", motor.Power);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            if (motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.AbsolutePosition || motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.Position || motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.PropertyField(PositionChanged);
            }
            if (motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.Speed || motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.PropertyField(SpeedChanged);
            }
            if (motor.Mode == TachoMotorWithAbsolutePosition.TachoMotorMode.Power)
            {
                EditorGUILayout.PropertyField(PowerChanged);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}