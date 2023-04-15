// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LEGOWirelessSDK
{
    public class HubBase : MonoBehaviour
    {
        #region serialized stufff
        public HubType hubType;
        public string connectToSpecificHubId;
        public bool autoConnectOnStart = true;
        [SerializeField] Color _ledColor = Color.green;
        public Color LedColor
        {
            get => _ledColor;
            set
            {
                _ledColor = value;
                if (IsConnected)
                {
                    rgbLight.SetColor(_ledColor);
                }
            }
        }
        #endregion
        #region non serialized state
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (value != _isConnected)
                {
                    _isConnected = value;
                    IsConnectedChanged.Invoke(_isConnected);
                }
            }
        }
        public string HubName { get { return device == null ? "" : device.DeviceName; } }
        private bool _buttonPressed;
        public bool ButtonPressed { get => _buttonPressed; private set { _buttonPressed = value; ButtonChanged.Invoke(_buttonPressed); } }
        public int BatteryLevel { get { return device == null ? 0 : device.BatteryLevel; } }
        #endregion
        #region services
        public List<ServiceBase> internalServices = new List<ServiceBase>();
        public List<ServiceBase> externalServices = new List<ServiceBase>();
        #endregion
        #region events
        public UnityEvent<bool> IsConnectedChanged;
        public UnityEvent<bool> ButtonChanged;
        #endregion

        #region internals
        public enum HubType
        {
            CityHub, TechnicHub, BoostHub, CityRemoteControl, SPIKEEssentialHub, DUPLOTrain
        }
        public ILEGODevice device { get; private set; }
        private RGBLight rgbLight = new RGBLight();

        private static List<ILEGODevice> usedDevices = new List<ILEGODevice>();
        private static int scanningCount = 0;

        private bool wantScan;

        void Awake()
        {
            if (LEGODeviceManager.Instance == null)
            {
                LEGODeviceManager.Initialize();
            }

            // Request user location service permission on Android.
            AndroidPermissionsHandler.RequestUserLocationPermission();
        }

        void Start()
        {
            if (autoConnectOnStart)
            {
                StartCoroutine(DelayedAutoConnect());
            }
        }

        void OnDestroy()
        {
            UseDevice(null);
            RequestScan(false);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Undo.RevertAllInCurrentGroup();
            var undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Reset Hub");
            SetHubType(HubType.TechnicHub, true, true);
            connectToSpecificHubId = null;
            autoConnectOnStart = true;
            _ledColor = Color.green;
            IsConnectedChanged = null;
            ButtonChanged = null;
            Undo.CollapseUndoOperations(undoGroup);
        }

        public void SetHubType(HubBase.HubType type, bool undo, bool force = false)
        {
            if (!force && type == hubType)
            {
                return;
            }

            if(undo)
            {
                Undo.RegisterCompleteObjectUndo(this, string.Empty);
            }

            hubType = type;

            foreach (ServiceBase s in internalServices)
            {
                if (s != null && s.gameObject == gameObject)
                {
                    if(undo)
                    {
                        Undo.DestroyObjectImmediate(s);
                    }
                    else
                    {
                        DestroyImmediate(s);
                    }
                }
            }
            foreach (ServiceBase s in externalServices)
            {
                if (s != null && s.transform.parent == transform)
                {
                    if (undo)
                    {
                        Undo.DestroyObjectImmediate(s.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(s.gameObject);
                    }
                }
            }

            var hubSerializedObject = new SerializedObject(this);
            var internalServicesProperty = hubSerializedObject.FindProperty("internalServices");
            hubSerializedObject.FindProperty("externalServices").ClearArray();
            internalServicesProperty.ClearArray();

            switch (type)
            {
                case HubBase.HubType.CityHub:
                    {
                        // No internal services.
                    }
                    break;
                case HubBase.HubType.TechnicHub:
                    {
                        AddInternalService<AccelerationSensor>(internalServicesProperty, 97, undo);
                        AddInternalService<OrientationSensor>(internalServicesProperty, 99, undo);
                        AddInternalService<TemperatureSensor>(internalServicesProperty, 61, undo);
                        AddInternalService<TemperatureSensor>(internalServicesProperty, 96, undo);
                    }
                    break;
                case HubBase.HubType.BoostHub:
                    {
                        AddInternalService<TiltSensorThreeAxis>(internalServicesProperty, 58, undo);
                        AddInternalService<TachoMotor>(internalServicesProperty, 56, undo, WriteDefaultInternalTachoMotorValues);
                        AddInternalService<TachoMotor>(internalServicesProperty, 55, undo, WriteDefaultInternalTachoMotorValues);
                        AddInternalService<TachoMotor>(internalServicesProperty, 57, undo, serializedObject =>
                        {
                            WriteDefaultInternalTachoMotorValues(serializedObject);
                            serializedObject.FindProperty("virtualConn").boolValue = true;
                        });
                    }
                    break;
                case HubBase.HubType.CityRemoteControl:
                    {
                        AddInternalService<Buttons>(internalServicesProperty, 0, undo);
                        AddInternalService<Buttons>(internalServicesProperty, 1, undo);
                    }
                    break;
                case HubBase.HubType.SPIKEEssentialHub:
                    {
                        AddInternalService<AccelerationSensor>(internalServicesProperty, 97, undo);
                        AddInternalService<OrientationSensor>(internalServicesProperty, 99, undo);
                        AddInternalService<TemperatureSensor>(internalServicesProperty, 96, undo);
                    }
                    break;
                case HubBase.HubType.DUPLOTrain:
                    {
                        AddInternalService<ColorSensorDuplo>(internalServicesProperty, 18, undo);
                        AddInternalService<MoveSensor>(internalServicesProperty, 19, undo);
                        AddInternalService<SoundPlayer>(internalServicesProperty, 1, undo);
                        AddInternalService<Motor>(internalServicesProperty, 0, undo);
                    }
                    break;
            }

            if (undo)
            {
                hubSerializedObject.ApplyModifiedProperties();
            }
            else
            {
                hubSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        T AddInternalService<T>(in SerializedProperty arrayProperty, int port, bool undo, Action<SerializedObject> onWriteToObject = null) where T : ServiceBase
        {
            var service = undo ? Undo.AddComponent<T>(gameObject) : gameObject.AddComponent<T>();
            var serviceSerializedObject = new SerializedObject(service);
            serviceSerializedObject.FindProperty("isInternal").boolValue = true;
            serviceSerializedObject.FindProperty("port").intValue = port;

            onWriteToObject?.Invoke(serviceSerializedObject);
            if (undo)
            {
                serviceSerializedObject.ApplyModifiedProperties();
            }
            else
            {
                serviceSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize -1).objectReferenceValue = service;

            return service;
        }

        void WriteDefaultInternalTachoMotorValues(SerializedObject serializedObject)
        {
            serializedObject.FindProperty("mode").intValue = (int)TachoMotor.TachoMotorMode.Position;
            serializedObject.FindProperty("typeBitMask").intValue = (int)TachoMotor.MotorIOTypes.InternalMotorWithTacho;
        }
#endif

        private IEnumerator DelayedAutoConnect()
        {
            yield return new WaitForSeconds(3);
            FindDeviceToUse();
        }

        private void DoSetup()
        {
            device.OnServiceConnectionChanged += ServiceConnectionChange;
            IList<ILEGOService> freeServices = device.Services.ToList();
            rgbLight.Setup(freeServices);
            Setup(freeServices);
            IsConnected = true;
            ButtonPressed = device.ButtonPressed;
            rgbLight.SetColor(LedColor);
        }

        private void Setup(ICollection<ILEGOService> freeServices)
        {
            foreach (var service in internalServices.Concat(externalServices).Where(s => s != null).OrderByDescending(s => s.port))
            {
                service.Setup(freeServices);
            }
        }

        private void ServiceConnectionChange(ILEGODevice device, ILEGOService service, bool connected)
        {
            if (connected)
            {
                var s = new List<ILEGOService>() { service };
                Setup(s);
                if (s.Count == 1)
                {
                    Debug.LogWarning("unused service connected " + service.ServiceName + ", type " + service.ioType + ", on port " + service.ConnectInfo.PortID);
                }
            }
        }

        public List<int> AllPorts()
        {
            switch (hubType)
            {
                case HubType.CityHub: return new List<int>() { 0, 1 };
                case HubType.TechnicHub: return new List<int>() { 0, 1, 2, 3 };
                case HubType.BoostHub: return new List<int>() { 0, 1 };
                case HubType.CityRemoteControl: return new List<int>();
                case HubType.SPIKEEssentialHub: return new List<int> { 0, 1 };
                case HubType.DUPLOTrain: return new List<int>();
            }
            return new List<int>();
        }

        public List<int> FreePorts()
        {
            List<int> ports = AllPorts();
            foreach (ServiceBase s in externalServices)
            {
                if (s != null && s.port != -1)
                {
                    ports.Remove(s.port);
                }
            }
            ports.Insert(0, -1);
            return ports;
        }

        void OnButton(ILEGODevice device, bool pressed)
        {
            ButtonPressed = pressed;
        }

        private void UseDevice(ILEGODevice newDevice)
        {
            if (device != null)
            {
                IsConnected = false;
                device.OnServiceConnectionChanged -= ServiceConnectionChange;
                device.OnButtonStateUpdated -= OnButton;
                device.OnDeviceStateUpdated -= OnDeviceState;
                LEGODeviceManager.Instance.DisconnectDevice(device);
                usedDevices.Remove(device);
            }
            device = newDevice;
            if (device != null)
            {
                usedDevices.Add(device);
                device.OnButtonStateUpdated += OnButton;
                device.OnDeviceStateUpdated += OnDeviceState;
                LEGODeviceManager.Instance.ConnectDevice(device);
            }
        }

        private void RequestScan(bool scan)
        {
            if (wantScan == scan)
            {
                return;
            }
            wantScan = scan;
            if (wantScan)
            {
                scanningCount++;
                LEGODeviceManager.Instance.OnDeviceStateUpdated += CheckNewDevice;
                if (LEGODeviceManager.Instance.ScanState == DeviceManagerState.NotScanning)
                {
                    LEGODeviceManager.Instance.Scan();
                }
            }
            else
            {
                scanningCount--;
                LEGODeviceManager.Instance.OnDeviceStateUpdated -= CheckNewDevice;
                if (scanningCount == 0 && LEGODeviceManager.Instance.ScanState != DeviceManagerState.NotScanning)
                {
                    LEGODeviceManager.Instance.StopScanning();
                }
            }
        }

        public void FindDeviceToUse()
        {
            if (!CheckAdvertisingDevices())
            {
                RequestScan(true);
            }
        }

        private bool CheckAdvertisingDevices()
        {
            foreach (var d in LEGODeviceManager.Instance.DevicesInState(DeviceState.DisconnectedAdvertising).OrderBy(d => new RSSIComparer()))
            {
                if (TryPotentialDevice(d))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckNewDevice(ILEGODevice d, DeviceState oldState, DeviceState newState)
        {
            if (d.State == DeviceState.DisconnectedAdvertising)
            {
                if (TryPotentialDevice(d))
                {
                    RequestScan(false);
                }
            }
        }

        private AbstractLEGODevice.DeviceType DeviceType()
        {
            switch (hubType)
            {
                case HubType.CityHub: return AbstractLEGODevice.DeviceType.Hub65;
                case HubType.TechnicHub: return AbstractLEGODevice.DeviceType.Hub128;
                case HubType.BoostHub: return AbstractLEGODevice.DeviceType.Hub64;
                case HubType.CityRemoteControl: return AbstractLEGODevice.DeviceType.Hub66;
                case HubType.SPIKEEssentialHub: return AbstractLEGODevice.DeviceType.Hub131;
                case HubType.DUPLOTrain: return AbstractLEGODevice.DeviceType.Hub32;
            }
            return new AbstractLEGODevice.DeviceType(DeviceSystemType.Unknown, -1);
        }

        private bool TryPotentialDevice(ILEGODevice d)
        {
            AbstractLEGODevice.DeviceType deviceType = DeviceType();
            if ((d.SystemType == deviceType.DeviceSystemType)
             && (d.SystemDeviceNumber == deviceType.SystemDeviceNumber)
             && !usedDevices.Contains(d)
             && (connectToSpecificHubId?.Length > 0 ? d.DeviceID == connectToSpecificHubId : true)
            )
            {
                UseDevice(d);
                return true;
            }
            return false;
        }

        private void OnDeviceState(ILEGODevice device, DeviceState oldState, DeviceState newState)
        {
            if (newState == DeviceState.InterrogationFinished)
            {
                DoSetup();
            }
            else if (newState == DeviceState.InterrogationFailed || newState == DeviceState.DisconnectedNotAdvertising || newState == DeviceState.DisconnectedAdvertising)
            {
                Debug.LogError("Device disconnected. State: " + device.State);
                UseDevice(null);
            }
        }

        public class RSSIComparer : Comparer<ILEGODevice>
        {
            public override int Compare(ILEGODevice x, ILEGODevice y)
            {
                return x.RSSIValue.CompareTo(y.RSSIValue);
            }
        }
        #endregion
    }
}