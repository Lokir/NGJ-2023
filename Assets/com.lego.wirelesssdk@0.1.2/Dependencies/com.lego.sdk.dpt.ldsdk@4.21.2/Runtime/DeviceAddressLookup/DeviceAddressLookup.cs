using System;
using System.Collections.Generic;
using System.Linq;
using LEGO.Logger;
using UnityEngine;

namespace LEGODeviceUnitySDK
{
    public class DeviceAddressLookup : IDeviceAddressLookup
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DeviceAddressLookup));

        public event Action OnNewAddress;

        private const int hardMaxAddressLimit = 4;
        //Setting soft max address limit
        private int addressLimit;

        //Dictionary of known addressable devices with <key> being an arbitrary address (e.g. 0-3) and <value> being the physical device guid.
        private Dictionary<int, string> legoDeviceAddresses;
        private readonly ILEGODeviceManager legoDeviceManager;

        public IEnumerable<string> AllKnownDeviceIds()
        {
            return legoDeviceAddresses.Values.Where(d => d != null);
        }

        [Obsolete("Constructor now takes an ILEGODeviceManager as its first parameter")]
        public DeviceAddressLookup(int maxAddresses)
        {
            throw new NotImplementedException(
                "You are calling the old constructor of DeviceAddressLookup, but its signature has changed.");
        }
        
        /// <param name="legoDeviceManager">The ILEGODeviceManager to use, when assessing connection status etc.</param>
        /// <param name="maxAddresses">The amount of devices that should be able to have an address. Must between 1 and 4 (inclusive).</param>
        public DeviceAddressLookup(ILEGODeviceManager legoDeviceManager, int maxAddresses = hardMaxAddressLimit)
        {
            this.legoDeviceManager = legoDeviceManager;
            
            //Calls to SetMaxAllowedDevices will initialize the AddressBook, so no need to do that explicitly
            SetMaxAllowedDevices(maxAddresses);
        }

        private void InitializeAddressBook()
        {
            string TryToFindDeviceId(int address)
            {
                if (legoDeviceAddresses == null)
                    return null;

                //Trying to reuse address
                legoDeviceAddresses.TryGetValue(address, out string deviceId);
                return deviceId;
            }

            legoDeviceAddresses = Enumerable.Range(0, addressLimit).ToDictionary(i => i, TryToFindDeviceId);
        }

        public void SetLegoDeviceOrder(Dictionary<int, ILEGODevice> newAddressOrder)
        {
            var newAddressOrderUsingIds =
                newAddressOrder.ToDictionary(item => item.Key, item => item.Value.DeviceID);
            
            SetLegoDeviceOrder(newAddressOrderUsingIds);
        }
        
        public void SetLegoDeviceOrder(Dictionary<int, string> newAddressOrder)
        {
            if (newAddressOrder.Any(kvp => kvp.Key > addressLimit - 1))
            {
                throw new NotSupportedException($"Address {newAddressOrder.Max(kvp => kvp.Key)} beyond max assignable address: {addressLimit - 1}.");
            }

            this.legoDeviceAddresses = new Dictionary<int, string>(addressLimit);
            for (var address = 0; address < addressLimit; address++)
            {
                legoDeviceAddresses[address] = newAddressOrder.ContainsKey(address) ? newAddressOrder[address] : null;
            }

            logger.Debug($"CC: Setting new order for lego device addresses");
#if(DEVELOPMENT_BUILD || UNITY_EDITOR)
            foreach (var legoDeviceAddress in newAddressOrder)
            {
                logger.Debug($"CC: {legoDeviceAddress.Value}: {legoDeviceAddress.Key}");
            }
#endif
        }

        public ILEGODevice GetLEGODeviceForAddress(int address)
        {
                legoDeviceAddresses.TryGetValue(address, out var deviceID);
                return (deviceID != null) ? legoDeviceManager.FindLegoDevice(deviceID) : null;
        }
        
        public string GetDeviceId(int address)
        {
            legoDeviceAddresses.TryGetValue(address, out var legoDevice);
            return legoDevice;
        }

        public (bool success, int address) TryGetAddressForLEGODevice(ILEGODevice legoDevice)
        {
            var success = TryGetAddressForLEGODevice(legoDevice, out var address);
            return (success, address);
        }

        public bool TryGetAddressForLEGODevice(ILEGODevice legoDevice, out int address)
        {
            address = -1;
            // could get actual LDSDK database reality to do check?
            ILEGODevice currentLDSDKLEGODevice = legoDeviceManager.FindLegoDevice(legoDevice.DeviceID);
            
            if (currentLDSDKLEGODevice == null)
                return false;

            if (currentLDSDKLEGODevice.State == DeviceState.DisconnectedNotAdvertising)
            {
                logger.Debug(
                    $"CC: {currentLDSDKLEGODevice.DeviceName} hash:{currentLDSDKLEGODevice.GetHashCode()}: Will not assign an address to a device which is disconnectedNotAdvertising");
                return false;
            }

            //Check if lego device already has an address
            if(TryLookupDeviceAddress(currentLDSDKLEGODevice, out address))
            {
                legoDeviceAddresses[address] = currentLDSDKLEGODevice.DeviceID;
                logger.Debug($"CC: {currentLDSDKLEGODevice.DeviceName} hash:{currentLDSDKLEGODevice.GetHashCode()} : Already has address: {address}");
                return true;
            }

            //Check if all addresses are occupied
            if (legoDeviceAddresses.All(t => t.Value != null))
            {
                logger.Debug($"CC: All address slots are occupied, will see if one can be released");
                //If all addresses are occupied, check if there exist an addressed device that is not connected.
                if (TryGetLastAddressThatIsNotConnected(out address))
                {
                    logger.Debug($"CC: {currentLDSDKLEGODevice.DeviceName} hash:{currentLDSDKLEGODevice.GetHashCode()} is taking the place from Device with ID {legoDeviceAddresses[address]}  hash:{legoDeviceAddresses[address].GetHashCode()} at address {address}");
                    legoDeviceAddresses[address] = currentLDSDKLEGODevice.DeviceID;
                    OnNewAddress?.Invoke();
                    return true;
                }
                logger.Debug($"CC: All slots are occupied and connected, cannot supply an address for {currentLDSDKLEGODevice.DeviceName} hash:{legoDevice.GetHashCode()}");
                return false;
            }
            
            //There is an available slot. Lets find it.
            address = legoDeviceAddresses.First(t => t.Value == null).Key;
            legoDeviceAddresses[address] = currentLDSDKLEGODevice.DeviceID;
            logger.Debug($"CC: {currentLDSDKLEGODevice.DeviceName} hash:{currentLDSDKLEGODevice.GetHashCode()} Got address from an empty slot: {address}");
            OnNewAddress?.Invoke();
            
            return true;
        }
        
        private bool TryGetLastAddressThatIsNotConnected(out int address)
        {
            bool Disconnected(string deviceID)
            {
                var device = legoDeviceManager.FindLegoDevice(deviceID);
                if (device == null)
                    return true;

                return device.State == DeviceState.DisconnectedNotAdvertising || device.State == DeviceState.DisconnectedAdvertising;
            };
            
            var lastDisconnectedDeviceKvp = legoDeviceAddresses.LastOrDefault(t => Disconnected(t.Value));
            
            if (lastDisconnectedDeviceKvp.Value == null)
            {
                address = -1;
                return false;
            }

            address = lastDisconnectedDeviceKvp.Key;
            return true;
        }
        public bool DeviceHasAnAddress(ILEGODevice legoDevice)
        {
            return legoDevice != null && legoDeviceAddresses.ContainsValue(legoDevice.DeviceID);
        }

        public bool TryLookupDeviceAddress(ILEGODevice legoDevice, out int address)
        {
            foreach (var kvp in legoDeviceAddresses.Where(t => t.Value != null))
            {
                if (kvp.Value == legoDevice.DeviceID)
                {
                    address = kvp.Key;
                    return true;
                }
            }
            address = -1;
            return false;
        }

        public void SetMaxAllowedDevices(int maxDevices)
        {
            addressLimit = Mathf.Clamp(maxDevices, 1, hardMaxAddressLimit);

            if (maxDevices != addressLimit)
            {
                logger.Warn($"DeviceAddressLookup constructed with {maxDevices}. Clamped to {addressLimit}");
            }
            
            InitializeAddressBook();
        }

        public int GetMaxAllowedDevices()
        {
            return addressLimit;
        }

        public void RemoveDevices(IEnumerable<string> deviceIDsToRemove)
        {
            var iDsToRemove = deviceIDsToRemove.ToList();
            logger.Debug($"CC: Removing devices from lookup:");
            foreach (var id in iDsToRemove)
            {
                var address = legoDeviceAddresses.FirstOrDefault(t => t.Value != null && t.Value == id);
                if (address.Value == null) continue;
                logger.Debug($"CC: removing device with id. :{address.Value}");
                legoDeviceAddresses[address.Key] = null;
            }
        }
        
        [Obsolete("Checking whether device object has changed is no longer necessary; simply omit this call, and assume the result false.")]
        public bool CheckDeviceObjectChange(ILEGODevice legoDevice)
        {
            throw new NotImplementedException(
                "You are calling CheckDeviceObjectChange(...) which has been deprecated. See obsolete-attribute.");
        }
    }
}
