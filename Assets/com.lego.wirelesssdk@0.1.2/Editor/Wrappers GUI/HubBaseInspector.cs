// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LEGOWirelessSDK
{
    [CustomEditor(typeof(HubBase), true)]
    public class HubBaseInspector : Editor
    {
        [MenuItem("LEGO Tools/Create LEGO Hub", priority = 200)]
        [MenuItem("GameObject/Create LEGO Hub", priority = 0)]
        private static void CreateLEGOHubObject()
        {
            var hub = new GameObject("LEGO Hub");
            var hubBase = hub.AddComponent<HubBase>();
            hubBase.SetHubType(HubBase.HubType.TechnicHub, false);

            Undo.SetCurrentGroupName("Create LEGO Hub");
            Undo.RegisterCreatedObjectUndo(hub, string.Empty);
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

            Selection.activeGameObject = hub;
        }

        SerializedProperty autoConnectOnStart;
        SerializedProperty connectToSpecificDeviceId;
        SerializedProperty IsConnectedChanged;
        SerializedProperty ButtonChanged;

        Texture2D banner;
        GUIStyle bannerStyle;
        HubBase.HubType bannerType;
        Rect addButtonRect;

        void OnEnable()
        {
            autoConnectOnStart = serializedObject.FindProperty("autoConnectOnStart");
            connectToSpecificDeviceId = serializedObject.FindProperty("connectToSpecificHubId");
            IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
            ButtonChanged = serializedObject.FindProperty("ButtonChanged");
            if (bannerStyle == null)
            {
                bannerStyle = new GUIStyle
                {
                    padding = new RectOffset(0, 0, 10, 0),
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void LoadBanner(HubBase.HubType type)
        {
            if (banner == null || type != bannerType)
            {
                bannerType = type;
                string path = null;
                switch (type)
                {
                    case HubBase.HubType.CityHub: path = "City Hub"; break;
                    case HubBase.HubType.BoostHub: path = "Boost Hub"; break;
                    case HubBase.HubType.CityRemoteControl: path = "City Remote Control"; break;
                    case HubBase.HubType.TechnicHub: path = "Technic Hub"; break;
                    case HubBase.HubType.SPIKEEssentialHub: path = "SPIKE Essential Hub"; break;
                    case HubBase.HubType.DUPLOTrain: path = "DUPLO Train"; break;
                }
                if (path != null)
                {
                    banner = Resources.Load<Texture2D>(path);
                }
                else
                {
                    banner = null;
                }
            }
        }

        

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            HubBase hub = target as HubBase;

            LoadBanner(hub.hubType);
            if (banner != null)
            {
                GUILayout.Box(banner, bannerStyle, GUILayout.Height(200));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Connected Hub Id");
            EditorGUILayout.SelectableLabel(hub.device == null ? "Not Connected" : hub.device.DeviceID, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (hub.device != null)
            {
                if (GUILayout.Button(EditorGUIUtility.FindTexture("Clipboard"), GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    EditorGUIUtility.systemCopyBuffer = hub.device.DeviceID;
                }
            }
            EditorGUILayout.EndHorizontal();

            HubBase.HubType newType = (HubBase.HubType)EditorGUILayout.EnumPopup("Hub Type", hub.hubType);
            if(newType != hub.hubType)
            {
                Undo.SetCurrentGroupName("Change Hub Type");
                hub.SetHubType(newType, true);
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }

            EditorGUILayout.PropertyField(autoConnectOnStart);

            EditorGUILayout.PropertyField(connectToSpecificDeviceId);

            Color newColor = EditorGUILayout.ColorField("LED Color", hub.LedColor);
            if (newColor != hub.LedColor)
            {
                Undo.RegisterCompleteObjectUndo(hub, "Changed LED color");
                hub.LedColor = newColor;
            }

            GUILayout.Space(10);
            GUILayout.Label("Status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Connected", hub.IsConnected);
            EditorGUILayout.TextField("Name", hub.HubName);
            if (hub.hubType != HubBase.HubType.DUPLOTrain)
            {
                EditorGUILayout.Toggle("Button Pressed", hub.ButtonPressed);
            }
            EditorGUILayout.IntField("Battery", hub.BatteryLevel);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
            GUILayout.Label("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(IsConnectedChanged);
            if (hub.hubType != HubBase.HubType.DUPLOTrain)
            {
                EditorGUILayout.PropertyField(ButtonChanged);
            }

            if (hub.FreePorts().Count > 1 || hub.externalServices.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("External Services", EditorStyles.boldLabel);
                List<int> freePorts = hub.FreePorts();
                for (int i = 0; i < hub.externalServices.Count; i++)
                {
                    ServiceBase service = hub.externalServices[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(service, typeof(ServiceBase), true);
                    EditorGUI.EndDisabledGroup();
                    if (service != null)
                    {
                        int[] ports;
                        if (service.port != -1)
                        {
                            ports = freePorts.Append(service.port).ToArray();
                            Array.Sort(ports);
                        }
                        else
                        {
                            ports = freePorts.ToArray();
                        }
                        var firstPortName = hub.hubType == HubBase.HubType.BoostHub ? 'C' : 'A';
                        string[] portNames = ports.Select(p => p < 0 ? "Any" : ((char)(firstPortName + p)).ToString()).ToArray();
                        var newPort = EditorGUILayout.IntPopup(service.port, portNames, ports, GUILayout.Width(60));
                        if (newPort != service.port)
                        {
                            Undo.RegisterCompleteObjectUndo(service, "Changed port");
                            service.port = newPort;
                        }
                    }

                    if (GUILayout.Button(EditorGUIUtility.FindTexture("Toolbar Minus"), GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        Undo.RegisterCompleteObjectUndo(hub, "Remove external service");
                        if (hub.externalServices[i] != null)
                        {
                            Undo.DestroyObjectImmediate(hub.externalServices[i].gameObject);
                        }
                        hub.externalServices.RemoveAt(i--);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                if (hub.externalServices.Count < hub.AllPorts().Count)
                {
                    if (GUILayout.Button("Add External Service", GUILayout.Height(50)))
                    {
                        PopupWindow.Show(addButtonRect, new AddServicePopup(hub));
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        addButtonRect = GUILayoutUtility.GetLastRect();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    public class AddServicePopup : PopupWindowContent
    {
        private HubBase hub;
        public enum ServiceType
        {
            Motor,
            TachoMotor,
            TachoMotorWithAbsolutePosition,
            WhiteLight,
            LEDMatrix,
            VisionSensor,
            TiltSensor,
            MotionSensor,
            //ColorSensorDuplo,
            ColorSensorTechnic,
            DistanceSensor,
            ForceSensor
        }

        ServiceType serviceType;

        Enum mode;
        int port;
        GUIStyle bannerStyle;

        const int padding = 10;

        public AddServicePopup(HubBase hub)
        {
            this.hub = hub;
            port = -1;
            bannerStyle = new GUIStyle
            {
                padding = new RectOffset(0, 0, 10, 10),
                alignment = TextAnchor.MiddleCenter
            };

            SetType(ServiceType.TachoMotorWithAbsolutePosition);
        }

        private void SetType(ServiceType newType)
        {
            serviceType = newType;
            mode = null;
            if (newType == ServiceType.TachoMotor)
            {
                mode = TachoMotor.TachoMotorMode.Position;
            }
            else if (newType == ServiceType.TachoMotorWithAbsolutePosition)
            {
                mode = TachoMotorWithAbsolutePosition.TachoMotorMode.Position;
            }
            /*else if (newType == ServiceType.ColorSensorDuplo)
            {
                mode = ColorSensorDuplo.ColorSensorDuploMode.Color;
            }*/
            else if (newType == ServiceType.ColorSensorTechnic)
            {
                mode = ColorSensorTechnic.ColorSensorTechnicMode.Color;
            }
            else if (newType == ServiceType.VisionSensor)
            {
                mode = VisionSensor.VisionSensorMode.Color;
            }
            else if (newType == ServiceType.TiltSensor)
            {
                mode = TiltSensor.TiltSensorMode.Angle;
            }
            else if (newType == ServiceType.MotionSensor)
            {
                mode = MotionSensor.MotionSensorMode.Distance;
            }
            else if (newType == ServiceType.ForceSensor)
            {
                mode = ForceSensor.ForceSensorMode.Force;
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 250);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(new Rect(padding, padding, editorWindow.position.width - 2 * padding, editorWindow.position.height - 2 * padding));
            GUILayout.Box(Resources.Load<Texture2D>(GetImage(serviceType)), bannerStyle, GUILayout.Height(150));

            var newServiceType = (ServiceType)EditorGUILayout.EnumPopup(GUIContent.none, serviceType, CheckService, true);

            if (newServiceType != serviceType)
            {
                SetType(newServiceType);
            }

            if (mode != null)
            {
                mode = EditorGUILayout.EnumPopup("Mode", mode);
            }

            int[] ports = hub.FreePorts().ToArray();
            var firstPortName = hub.hubType == HubBase.HubType.BoostHub ? 'C' : 'A';
            string[] portNames = ports.Select(p => p < 0 ? "Any" : ((char)(firstPortName + p)).ToString()).ToArray();
            port = EditorGUILayout.IntPopup("Port", port, portNames, ports);

            if (GUILayout.Button("Add"))
            {
                GameObject serviceGO = new GameObject(ObjectNames.NicifyVariableName(serviceType.ToString()));
                serviceGO.transform.parent = hub.transform;
                ServiceBase service = serviceGO.AddComponent(GetType(serviceType)) as ServiceBase;
                service.port = port;
                if (service.GetType() == typeof(ColorSensorDuplo))
                {
                    (service as ColorSensorDuplo).Mode = (ColorSensorDuplo.ColorSensorDuploMode)mode;
                }
                else if (service.GetType() == typeof(ColorSensorTechnic))
                {
                    (service as ColorSensorTechnic).Mode = (ColorSensorTechnic.ColorSensorTechnicMode)mode;
                }
                else if (service.GetType() == typeof(VisionSensor))
                {
                    (service as VisionSensor).Mode = (VisionSensor.VisionSensorMode)mode;
                }
                else if (service.GetType() == typeof(TiltSensor))
                {
                    (service as TiltSensor).Mode = (TiltSensor.TiltSensorMode)mode;
                }
                else if (service.GetType() == typeof(TachoMotor))
                {
                    (service as TachoMotor).Mode = (TachoMotor.TachoMotorMode)mode;
                }
                else if (service.GetType() == typeof(TachoMotorWithAbsolutePosition))
                {
                    (service as TachoMotorWithAbsolutePosition).Mode = (TachoMotorWithAbsolutePosition.TachoMotorMode)mode;
                }
                else if (service.GetType() == typeof(ForceSensor))
                {
                    (service as ForceSensor).Mode = (ForceSensor.ForceSensorMode)mode;
                }
                Undo.RegisterCreatedObjectUndo(serviceGO, "Added service");
                Undo.RegisterCompleteObjectUndo(hub, "Added service");
                hub.externalServices.Add(service);

                editorWindow.Close();
            }

            GUILayout.EndArea();
        }

        bool CheckService(Enum service)
        {
            if (hub.hubType == HubBase.HubType.CityHub || hub.hubType == HubBase.HubType.BoostHub)
            {
                if ((ServiceType)service == ServiceType.LEDMatrix)
                {
                    return false;
                }
            }
            else if (hub.hubType == HubBase.HubType.SPIKEEssentialHub)
            {
                if ((ServiceType)service == ServiceType.WhiteLight)
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetImage(ServiceType type)
        {
            switch (type)
            {
                case ServiceType.ColorSensorTechnic: return "SPIKE Color Sensor";
                case ServiceType.ForceSensor: return "SPIKE Force Sensor";
                case ServiceType.DistanceSensor: return "SPIKE Distance Sensor";
                case ServiceType.VisionSensor: return "Boost Color Sensor";
                case ServiceType.TiltSensor: return "WeDo Tilt Sensor";
                case ServiceType.MotionSensor: return "WeDo Motion Sensor";
                case ServiceType.WhiteLight: return "City Lights";
                case ServiceType.LEDMatrix: return "Spike LED Matrix";
                case ServiceType.Motor: return "Motors";
                case ServiceType.TachoMotor: return "Boost Motor";
                case ServiceType.TachoMotorWithAbsolutePosition: return "Tacho Motors With Absolute Position";
                default: return "";
            }
        }

        public static Type GetType(ServiceType type)
        {
            switch (type)
            {
                case ServiceType.ColorSensorTechnic: return typeof(ColorSensorTechnic);
                case ServiceType.ForceSensor: return typeof(ForceSensor);
                case ServiceType.DistanceSensor: return typeof(DistanceSensor);
                case ServiceType.VisionSensor: return typeof(VisionSensor);
                case ServiceType.TiltSensor: return typeof(TiltSensor);
                case ServiceType.MotionSensor: return typeof(MotionSensor);
                case ServiceType.WhiteLight: return typeof(WhiteLight);
                case ServiceType.LEDMatrix: return typeof(LEDMatrix);
                case ServiceType.Motor: return typeof(Motor);
                case ServiceType.TachoMotor: return typeof(TachoMotor);
                case ServiceType.TachoMotorWithAbsolutePosition: return typeof(TachoMotorWithAbsolutePosition);
                default: return null;
            }
        }
    }
}