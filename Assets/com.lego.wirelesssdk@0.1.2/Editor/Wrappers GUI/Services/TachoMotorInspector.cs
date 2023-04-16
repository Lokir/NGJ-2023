// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using System;
using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(TachoMotor))]
    public class TachoMotorInspector : Editor
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
            TachoMotor motor = serializedObject.targetObject as TachoMotor;

            if (motor.isInternal && motor.port >= 55 && motor.port <= 57)
            {
                switch (motor.port)
                {
                    case 55: EditorGUILayout.LabelField("Right", EditorStyles.largeLabel); break;
                    case 56: EditorGUILayout.LabelField("Left", EditorStyles.largeLabel); break;
                    case 57: EditorGUILayout.LabelField("Virtual Dual", EditorStyles.largeLabel); break;
                }
            }

            var newMode = (TachoMotor.TachoMotorMode)EditorGUILayout.EnumPopup("Mode", motor.Mode);
            if (newMode != motor.Mode)
            {
                Undo.RegisterCompleteObjectUndo(motor, "Changed motor mode");
                motor.Mode = newMode;
            }

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", motor.IsConnected);
            if (motor.Mode == TachoMotor.TachoMotorMode.Position || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.IntField("Position", motor.Position);
            }
            if (motor.Mode == TachoMotor.TachoMotorMode.Speed || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.IntField("Speed", motor.Speed);

            }
            if (motor.Mode == TachoMotor.TachoMotorMode.Power)
            {
                EditorGUILayout.IntField("Power", motor.Power);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            if (motor.Mode == TachoMotor.TachoMotorMode.Position || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.PropertyField(PositionChanged);
            }
            if (motor.Mode == TachoMotor.TachoMotorMode.Speed || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
            {
                EditorGUILayout.PropertyField(SpeedChanged);
            }
            if (motor.Mode == TachoMotor.TachoMotorMode.Power)
            {
                EditorGUILayout.PropertyField(PowerChanged);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}