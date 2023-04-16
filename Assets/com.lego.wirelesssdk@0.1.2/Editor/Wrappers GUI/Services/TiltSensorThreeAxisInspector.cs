// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(TiltSensorThreeAxis))]
    public class TiltSensorThreeAxisInspector : Editor
    {
        SerializedProperty IsConnectedChanged;
        SerializedProperty AngleChanged;
        SerializedProperty TiltChanged;
        SerializedProperty ShakeChanged;
        SerializedProperty AccelerationChanged;

        void OnEnable()
        {
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
            AngleChanged = serializedObject.FindProperty("AngleChanged");
            TiltChanged = serializedObject.FindProperty("TiltChanged");
            ShakeChanged = serializedObject.FindProperty("ShakeChanged");
            AccelerationChanged = serializedObject.FindProperty("AccelerationChanged");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TiltSensorThreeAxis tilt = serializedObject.targetObject as TiltSensorThreeAxis;

            var newMode = (TiltSensorThreeAxis.TiltSensorThreeAxisMode)EditorGUILayout.EnumPopup("Mode", tilt.Mode);
            if (newMode != tilt.Mode)
            {
                Undo.RegisterCompleteObjectUndo(tilt, "Changed tilt sensor mode");
                tilt.Mode = newMode;
            }

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", tilt.IsConnected);
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Angle)
            {
                EditorGUILayout.Vector3Field("Angle", tilt.Angle);
            }
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Orientation)
            {
                EditorGUILayout.LabelField("Tilt", tilt.Tilt + " " + ((LETiltSensorThreeAxisOrientation)tilt.Tilt).ToString());
            }
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Impact)
            {
                EditorGUILayout.Toggle("Shake", tilt.Shake);
            }
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Acceleration)
            {
                EditorGUILayout.Vector3Field("Acceleration", tilt.Acceleration);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Angle)
            {
                EditorGUILayout.PropertyField(AngleChanged);
            }
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Orientation)
            {
                EditorGUILayout.PropertyField(TiltChanged);
            }
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Impact)
            {
                EditorGUILayout.PropertyField(ShakeChanged);
            }
            if (tilt.Mode == TiltSensorThreeAxis.TiltSensorThreeAxisMode.Acceleration)
            {
                EditorGUILayout.PropertyField(AccelerationChanged);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}