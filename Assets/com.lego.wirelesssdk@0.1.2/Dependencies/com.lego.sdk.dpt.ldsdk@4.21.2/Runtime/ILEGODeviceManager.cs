using System;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// This is the entry point for all access to LEGO BlE enabled devices.
    /// Use this manager to scan and connect to to advertising devices.
    ///
    /// You must initialize the LEGODeviceManager before calling Scan or any other method that invokes
    /// methods on the native side. The initialize method will add the LEGODeviceManager game-boject under DontDestroyOnLoad
    ///
    /// </summary>
    public interface ILEGODeviceManager
    {
        #region Scanning for Advertising Devices
        
        /// <summary>
        /// Start scanning for advertising LEGO BLE Devices
        /// </summary>
        void Scan();

        /// <summary>
        /// Stop scanning for advertising LEGO BLE Devices
        /// </summary>
        void StopScanning();

        /// <summary>
        /// Used to check bluetooth capbailities
        /// Note, if bluetooth is turned on but there is no Low Energy capabilities, this will return NotSupported.
        /// Hence, normally you will want to check for 'NotSupported' before checking for off.
        ///
        /// For iOS this property will always return Unknown - iOS will prompt the user to turn on Bluetooth itself if necessary.
        ///
        /// </summary>
        /// <value>The state of the bluetooth.</value>
        BluetoothState BluetoothState { get; }


        /// <summary>
        /// Get the current Scan State of the DeviceManager.
        /// </summary>
        DeviceManagerState ScanState { get; }

        /// <summary>
        /// Called whenever state changes from 'Scanning for' / 'Stopped scanning for' Devices
        /// Normally this will be triggered by a call to Scan or StopScanning, but it may also
        /// be triggered by the SDK itself, for instance in relation to updating firmware, where
        /// the SDK will force 'Scanning' to connect to the Bootloader.
        /// </summary>
        event Action<DeviceManagerState> OnScanStateUpdated;

        #endregion Scanning for Advertising Devices

        #region Discovering Devices

        /// <summary>
        /// Use this method to get all Devices in a specific state.
        /// </summary>
        List<ILEGODevice> DevicesInState(DeviceState deviceState, bool includeBootLoaderDevices = false);

        /// <summary>
        /// Register for this update to get nofications as LEGODevices start advertising, stops advertising,
        /// starts connecting, etc.
        ///
        /// Once you get an event for 'DisconnectedAdvertising' you may call Connect on that device.
        /// </summary>
        event Action<ILEGODevice, DeviceState /* old state */, DeviceState /* new state */> OnDeviceStateUpdated;

        #endregion Discovering Devices

        #region Connect / Disconnect to Devices

        /// <summary>
        /// Connect to a LEGO Device
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void ConnectToDevice(ILEGODevice legoDeviceID);

        /// <summary>
        /// Disconnect from a LEGO Device
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void DisconnectDevice(ILEGODevice legoDeviceID);
     
        /// <summary>
        /// Request the hub to power off.
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void RequestDeviceToSwitchOff(ILEGODevice legoDeviceID);
        
        /// <summary>
        /// Request the hub to disconnect. This will trigger the OnDeviceWillDisconnect delegate method in LEGOServiceCallbacks
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void RequestDeviceToDisconnect(ILEGODevice legoDeviceID);

                /// <summary>
        /// Called whenever the connection to a Device is closed, either by the user or due to a failure.
        /// The DeviceConnectReasons states why the connection was closed.
        /// </summary>
        event Action<ILEGODevice, DeviceDisconnectReason> OnDeviceDisconnected;

        #endregion Connect / Disconnect

        #region Getting known Devices

        event Action OnPacketSent;
        event Action OnPacketReceived;
        event Action OnPacketDropped;
        
        /// <summary>
        /// All devices currently known by the Manager.
        ///
        /// A device is added to this list once it is discovered advertising (state == DisconnectedAdvertising)
        /// A device is removed from this list once it stops advertising (state == DiconnectedNotAdvertising)
        ///
        /// </summary>
        ILEGODevice[] Devices { get; }

        /// <summary>
        /// Finds a lego device by its DeviceID. If no matching device is found, this returns null and logs a warning.
        /// </summary>
        ILEGODevice FindLegoDevice(string deviceID);

        /// <summary>
        /// If true, returns all connected devices, that is devices in state 'Connecting', 'Interrogating' or 'Interrogating finished'
        /// If false, returns all known disconnected devices, that is devices in state 'DisconnectedAdvertising' and 'DisconnectedNotAdvertising'
        /// </summary>
        List<ILEGODevice> DevicesConnected(bool connected);


        /// <summary>
        /// This method will syncrohize the Devices property with the list of devices maintained in the native SDK.
        /// The only real use of this method right now is to make sure we get a list that is also sorted according to most recent RSSI value.
        /// Otherwise, there should be no need to explicitely refresh the list of Devices as this is automatically maintaned based on DeviceState update events.
        /// </summary>
        void RefreshDeviceList();

        [Obsolete("DeviceAddressLookup has moved to the ConnectionCenter package, and is available through ConnectionCenter.GetDeviceAddressLookup().")]
        IDeviceAddressLookup DeviceAddressLookup { get; }

        #endregion Getting known Devices

        #region LEGO Wireless SDK Info

        /// <summary>
        /// Gets info about the SDK version and which Firmware.
        /// </summary>
        /// <value>The SDKV ersion info.</value>
        LEGODeviceSDKVersionInfo SDKVersionInfo { get; }

    #endregion LEGO Wireless SDK Info

    /// <summary>
    /// Deletes the persisted cache of mode information from persisted cache
    /// </summary>
    void ClearPersistedModeInformationCache();
    }

    public enum BluetoothState
    {
        Unknown,
        On,
        Off,
        MissingLocationPermission
    }
    public enum DeviceManagerState
    {
        ScanRequested = 0,
        Scanning = 1,
        NotScanning = 2,
        StopScanRequested = 3
    }
}

