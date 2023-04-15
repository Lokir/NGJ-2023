// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(TiltSensor))]
    public class TiltSensorInspector : Editor
    {
        SerializedProperty IsConnectedChanged;
        SerializedProperty AngleChanged;
        SerializedProperty TiltChanged;
        SerializedProperty ShakeChanged;

        void OnEnable()
        {
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
            AngleChanged = serializedObject.FindProperty("AngleChanged");
            TiltChanged = serializedObject.FindProperty("TiltChanged");
            ShakeChanged = serializedObject.FindProperty("ShakeChanged");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TiltSensor tilt = serializedObject.targetObject as TiltSensor;

            var newMode = (TiltSensor.TiltSensorMode)EditorGUILayout.EnumPopup("Mode", tilt.Mode);
            if (newMode != tilt.Mode)
            {
                Undo.RegisterCompleteObjectUndo(tilt, "Changed tilt sensor mode");
                tilt.Mode = newMode;
            }

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", tilt.IsConnected);
            if (tilt.Mode == TiltSensor.TiltSensorMode.Angle)
            {
                EditorGUILayout.Vector3Field("Angle", tilt.Angle);
            }
            if (tilt.Mode == TiltSensor.TiltSensorMode.Tilt)
            {
                EditorGUILayout.LabelField("Tilt", tilt.Tilt + " " + ((LEGOTiltSensor.LETiltSensorDirection)tilt.Tilt).ToString());
            }
            if (tilt.Mode == TiltSensor.TiltSensorMode.Crash)
            {
                EditorGUILayout.Toggle("Shake X", tilt.Shake.X);
                EditorGUILayout.Toggle("Shake Y", tilt.Shake.Y);
                EditorGUILayout.Toggle("Shake Z", tilt.Shake.Z);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            if (tilt.Mode == TiltSensor.TiltSensorMode.Angle)
            {
                EditorGUILayout.PropertyField(AngleChanged);
            }
            if (tilt.Mode == TiltSensor.TiltSensorMode.Tilt)
            {
                EditorGUILayout.PropertyField(TiltChanged);
            }
            if (tilt.Mode == TiltSensor.TiltSensorMode.Crash)
            {
                EditorGUILayout.PropertyField(ShakeChanged);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}