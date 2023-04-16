using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using LEGO.Logger;
using CoreUnityBleBridge;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public class LEGODeviceManager : MonoBehaviour, ILEGODeviceManager
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGODeviceManager));

#pragma warning disable 067
        public event Action<DeviceManagerState> OnScanStateUpdated = delegate {};
        public event Action<ILEGODevice, DeviceState, DeviceState> OnDeviceStateUpdated;
        public event Action<ILEGODevice, DeviceDisconnectReason> OnDeviceDisconnected;

        public event Action OnPacketSent;
        public event Action OnPacketReceived;
        public event Action OnPacketDropped;
#pragma warning restore 067

        private ILEGODeviceConfig lEGODeviceConfig;

        private static GameObject deviceManagerGameObject;
        public static LEGODeviceManager Instance;

        private IBleBridge bleBridge;

#region Sub-states
        public DeviceManagerState ScanState { get { return TranslateAdapterState(bleBridge.AdapterScanState); } }
        private DeviceManagerState TranslateAdapterState(AdapterScanState rawState)
        {
            switch (rawState) 
            {
                case AdapterScanState.TurningOnScanning:
                    return DeviceManagerState.ScanRequested;
                case AdapterScanState.Scanning:
                    switch (scanCommandSentStatus)
                    {
                        case ScanCommandSentStatus.StopScanSent:
                            return DeviceManagerState.StopScanRequested;
                        default:
                        case ScanCommandSentStatus.StartScanSent:
                        case ScanCommandSentStatus.NoOutstandingCommand: 
                            return DeviceManagerState.Scanning;
                    }

                case AdapterScanState.NotScanning:
                    switch (scanCommandSentStatus)
                    {
                        case ScanCommandSentStatus.StartScanSent:
                            return DeviceManagerState.ScanRequested;
                        default:
                        case ScanCommandSentStatus.NoOutstandingCommand:
                        case ScanCommandSentStatus.StopScanSent:
                            return DeviceManagerState.NotScanning;
                    }
 
                case AdapterScanState.BluetoothUnavailable:
                case AdapterScanState.BluetoothDisabled:
                default:
                    return DeviceManagerState.NotScanning;
            }
        }

        private DeviceListTracker deviceListTracker = new DeviceListTracker();
#endregion

        #region Lifecycle
        //Should be called before ANY messages communicating the native libs
        public static ILEGODeviceManager Initialize(ILEGODeviceConfig lEGODeviceConfig = null)
        {
            if (Instance != null)
            {
                logger.Warn("Ignoring call to Initialize for LEGODeviceManager - already initialized");
                return Instance;
            }

            deviceManagerGameObject = new GameObject("LEGODeviceManager");

            Instance = deviceManagerGameObject.AddComponent<LEGODeviceManager>();
            DontDestroyOnLoad(deviceManagerGameObject);
            Instance.lEGODeviceConfig = lEGODeviceConfig ?? new LEGODeviceConfig();
            MainThreadDispatcher.Initialize();
            
            var bleFilters = new BleSettings.Filtering
            {
                GattServices = GattService.AllServices
            };
            
            var bleSettings = new BleSettings(bleFilters);
            BleFactory.Initialize(bleSettings, ((LEGODeviceManager) Instance).OnBleBridgeInitialized);

            return Instance;
        }

        private void OnBleBridgeInitialized (InitializationEventArgs args)
        {
            this.bleBridge = args.BleBridge;

            // Setup callbacks:
            bleBridge.ErrorOccurred += (msg => logger.Error("BLE Bridge error: "+msg));
            bleBridge.AdapterScanStateChanged += (scanState) =>
            {
                scanCommandSentStatus = ScanCommandSentStatus.NoOutstandingCommand;
                OnScanStateUpdated?.Invoke(ScanState);
            };

            bleBridge.DeviceAppeared += bleBridge => deviceListTracker.OnDeviceAppeared(bleBridge, lEGODeviceConfig);
            bleBridge.DeviceDisappeared += deviceListTracker.OnDeviceDisappeared;

            bleBridge.PacketSent += PacketSent; 
            bleBridge.PacketReceived += PacketReceived;
            bleBridge.PacketDropped += PacketDropped;
        }

        private void PacketSent()
        {
            OnPacketSent?.Invoke();
        }
        private void PacketReceived()
        {
            OnPacketReceived?.Invoke();
        }
        private void PacketDropped()
        {
            OnPacketDropped?.Invoke();
        }
        public static void Dispose()
        {
            Instance = null;
            Destroy(deviceManagerGameObject);
            //NativeBridge.Dispose();
        }

        public void Awake()
        {
            logger.Info("Calling Awake on LEGODeviceManager");
        }

        #endregion

        #region Device list
        public ILEGODevice[] Devices {
            get { return deviceListTracker.DevicesAsArray; }
        }
        public ILEGODevice FindLegoDevice(string deviceID)
        {
            return deviceListTracker.FindLegoDevice(deviceID);
        }

        public List<ILEGODevice> DevicesInState(DeviceState deviceState, bool includeBootLoaderDevices = false)
        {
            return deviceListTracker.DevicesInState(deviceState, includeBootLoaderDevices);
        }

        public List<ILEGODevice> DevicesConnected(bool connected)
        {
            return deviceListTracker.DevicesConnected(connected);
        }
        #endregion

        public LEGODeviceSDKVersionInfo SDKVersionInfo
        {
            get
            {
                return new LEGODeviceSDKVersionInfo(
                    version:     BuildConfig.SDK_VERSION,
                    buildNumber: BuildConfig.SDK_BUILD_NUMBER,
                    commit:      BuildConfig.SDK_GIT_HEAD_HASH,
                    v2FirmwareVersion: BuildConfig.SDK_TESTED_WITH_V2_FIRMWARE_VERSION,
                    v3FirmwareVersion: BuildConfig.SDK_TESTED_WITH_V3_FIRMWARE_VERSION);
            }
        }

        public BluetoothState BluetoothState
        {
            get
            {
                switch (bleBridge.AdapterScanState) {
                case AdapterScanState.NotScanning:
                case AdapterScanState.Scanning:
                case AdapterScanState.TurningOnScanning:
                    return BluetoothState.On;
                case AdapterScanState.BluetoothDisabled:
                case AdapterScanState.BluetoothUnavailable:
                default:
                    return BluetoothState.Off;
                }
            }
        }

        #region Discovery
        private enum ScanCommandSentStatus
        {
            StartScanSent,
            StopScanSent,
            NoOutstandingCommand
        }

        private ScanCommandSentStatus scanCommandSentStatus = ScanCommandSentStatus.NoOutstandingCommand;
        
        public void Scan() 
        {
            if (scanCommandSentStatus == ScanCommandSentStatus.StartScanSent)
            {
                return;
            }
            
            OnScanStateUpdated?.Invoke(DeviceManagerState.ScanRequested);

            bleBridge.SetScanState(true);
            scanCommandSentStatus = ScanCommandSentStatus.StartScanSent;
            logger.Info("LEGODeviceManager::scan StartScanSent");
        }
        
        public void StopScanning() 
        {
            if (scanCommandSentStatus == ScanCommandSentStatus.StopScanSent)
            {
                return;
            }
            OnScanStateUpdated?.Invoke(DeviceManagerState.StopScanRequested);
            
            bleBridge.SetScanState(false);
            scanCommandSentStatus = ScanCommandSentStatus.StopScanSent;
            logger.Info("LEGODeviceManager::scan StartScanSent");
        }
        #endregion

        #region Connection establishment
        public void ConnectToDevice(ILEGODevice legoDevice)
        {
            logger.Warn("Connect to device");
            ConnectToDevice(legoDevice.DeviceID);
        }

        public void ConnectToDevice(string legoDeviceID)
        {
            ILEGODevice device = deviceListTracker.FindLegoDevice(legoDeviceID);
            if (device != null)
                ConnectDevice(device);
            else
                logger.Warn("Connect requested on unknown device \""+legoDeviceID+"\"");
        }

        public void DisconnectDevice(string legoDeviceID)
        {
            ILEGODevice device = deviceListTracker.FindLegoDevice(legoDeviceID);
            if (device != null)
                DisconnectDevice(device);
            else
                logger.Warn("Disconnect requested on unknown device \""+legoDeviceID+"\"");
        }

        public void ConnectDevice(ILEGODevice legoDevice)
        {
            if (legoDevice is AbstractLEGODevice)
                ((AbstractLEGODevice)legoDevice).Connect();
            else
                logger.Warn("Connect requested on device of unknown type "+legoDevice.GetType().Name);
        }

        public void DisconnectDevice(ILEGODevice legoDevice)
        {
            if (legoDevice is AbstractLEGODevice)
                ((AbstractLEGODevice)legoDevice).Disconnect();
            else
                logger.Warn("Disconnect requested on device of unknown type "+legoDevice.GetType().Name);
        }

        internal void DeviceDidDisconnect(ILEGODevice device, DeviceDisconnectReason reason) 
        {
            OnDeviceDisconnected?.Invoke(device, reason);
        }            

        #endregion

        public void RequestDeviceToSwitchOff(ILEGODevice legoDevice)
        {
            var device = legoDevice as LEGODevice;
            if (device != null)
                ((LEGODevice)device).RequestToSwitchOff();
            else
                logger.Warn("RequestDeviceToSwitchOff requested on device of unknown type "+legoDevice.GetType().Name);
        }

        public void RequestDeviceToDisconnect(ILEGODevice legoDevice)
        {
            var device = legoDevice as LEGODevice;
            if (device != null)
                ((LEGODevice)device).RequestToDisconnect();
            else
                logger.Warn("RequestDeviceToDisconnect requested on device of unknown type "+legoDevice.GetType().Name);
        }

        public void ClearPersistedModeInformationCache()
        {
            InterrogationCacheManager.Instance.ClearCache();
        }


        public void RefreshDeviceList()
        {
            deviceListTracker.OrderDevicesByRSSI();
        }

        [Obsolete("DeviceAddressLookup has moved to the ConnectionCenter package, and is available through ConnectionCenter.GetDeviceAddressLookup().")]
        public IDeviceAddressLookup DeviceAddressLookup => null;


        /// Forward the device update state from the Device
        internal void HandleDeviceStateUpdated(ILEGODevice legoDevice, DeviceState oldState, DeviceState newState)
        {
            OnDeviceStateUpdated?.Invoke(legoDevice, oldState, newState);
        }
    }
}
