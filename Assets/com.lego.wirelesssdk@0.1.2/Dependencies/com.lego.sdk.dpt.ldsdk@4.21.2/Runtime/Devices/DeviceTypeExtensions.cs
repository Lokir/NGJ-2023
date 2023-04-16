using System;
using System.Collections.Generic;
using System.Linq;
using DeviceType = LEGODeviceUnitySDK.AbstractLEGODevice.DeviceType;

namespace LEGODeviceUnitySDK
{
    public static class DeviceTypeExtensions
    {
        private static readonly List<(string name, DeviceType type)> knownDevices = new List<(string, DeviceType)>
        {
            ("Hub32",  DeviceType.Hub32 ),
            ("Hub64",  DeviceType.Hub64 ),
            ("Hub65",  DeviceType.Hub65 ),
            ("Hub66",  DeviceType.Hub66 ),
            ("Hub128", DeviceType.Hub128),
            ("Hub131", DeviceType.Hub131)
        };

        public static DeviceType DeviceTypeFromString(this string deviceName)
        {
            var device = knownDevices.FirstOrDefault(t => t.name.ToLower().Equals(deviceName.ToLower()));

            if (device != default)
                return device.type;

            throw new Exception($"Unkown device {deviceName}, add it in DeviceTypeExtensions");
        }

        public static string StringFromDeviceType(this DeviceType deviceType)
        {
            var device = knownDevices.FirstOrDefault(t => t.type == deviceType);

            if (device != default)
                return device.name;

            throw new Exception($"Unkown device {deviceType}, add it in DeviceTypeExtensions");
        }
    }
}