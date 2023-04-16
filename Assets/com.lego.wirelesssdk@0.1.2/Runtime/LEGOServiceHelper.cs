// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;

namespace LEGOWirelessSDK
{
    public static class LEGOServiceHelper
    {
        public static IEnumerable<ILEGOService> GetServicesOfType(ILEGODevice device, IOType type)
        {
            return device.Services.Where(service => { return service.ioType == type; });
        }

        public static IEnumerable<ILEGOService> GetServicesInTypeCollection(ILEGODevice device, ICollection<IOType> types)
        {
            return device.Services.Where(service => { return types.Contains(service.ioType); });
        }

        public static IEnumerable<ILEGOService> GetServicesInTypeCollectionOnPort(ILEGODevice device, ICollection<IOType> types, int port)
        {
            return device.Services.Where(service => { return types.Contains(service.ioType) && service.ConnectInfo.PortID == port; });
        }

        public static IEnumerable<ILEGOService> GetServicesOnPort(ILEGODevice device, int port)
        {
            return device.Services.Where(service => { return service.ConnectInfo.PortID == port; });
        }

        public static IEnumerable<ILEGOService> GetServicesOfTypeOnPort(ILEGODevice device, IOType type, int port)
        {
            return device.Services.Where(service => { return service.ioType == type && service.ConnectInfo.PortID == port; });
        }
    }
}

