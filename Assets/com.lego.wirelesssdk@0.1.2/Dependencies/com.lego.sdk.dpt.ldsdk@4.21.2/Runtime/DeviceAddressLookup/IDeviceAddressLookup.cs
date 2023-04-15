using System;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{
    public interface IDeviceAddressLookup
    {
        /// <summary>
        /// This is invoked when a new address is assigned to a device
        /// </summary>
        event Action OnNewAddress;

        /// <summary>
        /// Returns all ILEGODevice with an address
        /// </summary>
        IEnumerable<string> AllKnownDeviceIds();

        /// <summary>
        /// Returns whether or not an ILEGODevice has an address
        /// </summary>
        bool DeviceHasAnAddress(ILEGODevice legoDevice);

        /// <summary>
        /// Returns the ILEGODevice on the given address
        /// </summary>
        /// <returns>Returns null if the device hasn't an address</returns>
        ILEGODevice GetLEGODeviceForAddress(int address);

        /// <summary>
        /// Removes all addresses in the look up, that is in the IEnumerable
        /// </summary>
        void RemoveDevices(IEnumerable<string> deviceIDsToRemove);

        /// <summary>
        /// This sets all devices and their addresses.
        /// Discarding whatever addresses there was before.
        /// The key in the dictionary is the zero-indexed address
        /// </summary>
        void SetLegoDeviceOrder(Dictionary<int, ILEGODevice> newAddressOrder);

        /// <summary>
        /// Overload of above method, using device GUIDs as dictionary-values instead of ILEGODevices.
        /// </summary>
        void SetLegoDeviceOrder(Dictionary<int, string> newAddressOrder);
        
        /// <summary>
        /// This method tries to assign an address to an ILEGODevice.
        /// if the returned value `success` is false, then the device didn't get an address.
        /// Perhaps because of max allowed connected devices was reached.
        /// </summary>
        (bool success, int address) TryGetAddressForLEGODevice(ILEGODevice legoDevice);

        /// <summary>
        /// Overload of above method
        /// </summary>
        bool TryGetAddressForLEGODevice(ILEGODevice legoDevice, out int address);

        /// <summary>
        /// Tries to find the address for a given ILEGODevice
        /// </summary>
        bool TryLookupDeviceAddress(ILEGODevice legoDevice, out int address);

        /// <summary>
        /// Sets the max allowed connected devices
        /// </summary>
        /// <param name="maxDevices">max devices allowed to connect to</param>
        void SetMaxAllowedDevices(int maxDevices);

        /// <summary>
        /// Gets the max allowed connected devices
        /// </summary>
        /// <returns>max devices allowed to connect to</returns>
        int GetMaxAllowedDevices();

        [Obsolete("Checking whether device object has changed is no longer necessary; simply omit this call, and assume the result false.")]
        bool CheckDeviceObjectChange(ILEGODevice legoDevice);
    }
}