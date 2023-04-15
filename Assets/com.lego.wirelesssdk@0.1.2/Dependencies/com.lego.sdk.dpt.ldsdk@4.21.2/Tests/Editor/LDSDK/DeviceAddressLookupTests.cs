using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LEGODeviceUnitySDK;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor;

namespace LEGO.LDSDK.Editor.Tests
{
    [TestFixture]
    public class DeviceAddressLookupTests
    {
        private IDeviceAddressLookup deviceAddressLookup;
        private ILEGODeviceManager fakeDeviceManager;
        [SetUp]
        public void Setup()
        {
            fakeDeviceManager = Substitute.For<ILEGODeviceManager>();
            deviceAddressLookup = new DeviceAddressLookup(fakeDeviceManager, 4);
        }

        [Test]
        public void AllKnownDevices_DevicesAdded_TryingToSetMoreThanAllowedDevices()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            var dev2 = Substitute.For<ILEGODevice>();
            var dev3 = Substitute.For<ILEGODevice>();
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2},
                {2, dev3}
            };
            deviceAddressLookup.SetMaxAllowedDevices(2);
            
            Assert.That(() => deviceAddressLookup.SetLegoDeviceOrder(devs),
                        Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void AllKnownDevices_DevicesAdded_ReturnsAllDevicesInput()
        {
            var dev1 = Guid.NewGuid().ToString();
            var dev2 = Guid.NewGuid().ToString();
            var dev3 = Guid.NewGuid().ToString();
            var dev4 = Guid.NewGuid().ToString();
            var devs = new Dictionary<int, string>()
            {
                {0, dev1},
                {1, dev2},
                {2, dev3},
                {3, dev4}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            var result = deviceAddressLookup.AllKnownDeviceIds();
            foreach (var device in result)
            {
                Assert.True(devs.ContainsValue(device));
            }
        }

        [Test]
        public void AllKnownDevices_NoDevicesAdded_ReturnsEmptyList()
        {
            var devs = new Dictionary<int, ILEGODevice>();
            
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            var result = deviceAddressLookup.AllKnownDeviceIds();
            Assert.False(result.Any());
        }

        [Test]
        public void TryLookupAddress_DeviceExists_ReturnsAddress()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            Assert.True(deviceAddressLookup.TryLookupDeviceAddress(dev1, out var address1));
            Assert.True(address1 == 0);
            Assert.True(deviceAddressLookup.TryLookupDeviceAddress(dev2, out var address2));
            Assert.True(address2 == 1);
        }
        
        [Test]
        public void TryLookupAddress_DeviceDoesNotExists_ReturnsFalse()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");

            Assert.False(deviceAddressLookup.TryLookupDeviceAddress(dev1, out _));
        }
        
        [Test]
        public void RemoveDevices_DeviceExists_NoDevicesAfter()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2}
            };
            var ids = devs.Select(p => p.Value.DeviceID);
            deviceAddressLookup.SetLegoDeviceOrder(devs);
            
            deviceAddressLookup.RemoveDevices(ids);

            Assert.False(deviceAddressLookup.TryLookupDeviceAddress(dev1, out _));
            Assert.False(deviceAddressLookup.TryLookupDeviceAddress(dev2, out _));
        }
        
        [Test]
        public void RemoveDevices_NoDevicesExists_NoDevicesAfter()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            
            deviceAddressLookup.RemoveDevices(new []{dev1.DeviceID});

            Assert.False(deviceAddressLookup.TryLookupDeviceAddress(dev1, out _));
        }
        
        [Test]
        public void DeviceHasAnAddress_DeviceExists_ReturnsTrue()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);
            
            Assert.True(deviceAddressLookup.DeviceHasAnAddress(dev1));
            Assert.True(deviceAddressLookup.DeviceHasAnAddress(dev2));
        }
        
        [Test]
        public void DeviceHasAnAddress_DeviceDoesNotExists_ReturnsFalse()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {1, dev2}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);
            
            Assert.False(deviceAddressLookup.DeviceHasAnAddress(dev1));
        }
        
        [Test]
        public void TryGetAddress_DeviceDoesNotExists_ReturnsNewAddress()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");

            Assert.True(deviceAddressLookup.TryGetAddressForLEGODevice(dev1, out var address));
            Assert.True(address != -1);
        }
        
        [Test]
        public void TryGetAddress_DeviceDoesNotExists_CallsEvent()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");

            fakeDeviceManager.FindLegoDevice(dev1.DeviceID).Returns(dev1);
            
            var eventFired = false;
            deviceAddressLookup.OnNewAddress += () => eventFired = true;
            
            deviceAddressLookup.TryGetAddressForLEGODevice(dev1, out _);
            
            Assert.True(eventFired);
        }
        
        [Test]
        public void TryGetAddress_DeviceExists_ReturnsExistingAddress()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");

            fakeDeviceManager.FindLegoDevice(dev1.DeviceID).Returns(dev1);
            
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            Assert.True(deviceAddressLookup.TryGetAddressForLEGODevice(dev1, out var address));
            Assert.True(address == 0);
        }
        
        [Test]
        public void TryGetAddress_DeviceExists_DoesNotCallEvent()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            
            fakeDeviceManager.FindLegoDevice(dev1.DeviceID).Returns(dev1);
            
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            var eventFired = false;
            deviceAddressLookup.OnNewAddress += () => eventFired = true;
            
            deviceAddressLookup.TryGetAddressForLEGODevice(dev1, out var address);
            
            Assert.False(eventFired);
        }
        
        [Test]
        public void TryGetAddress_OtherConnectedDeviceExists_ReturnsFreeAddress()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            dev1.State.Returns(DeviceState.InterrogationFinished);
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            
            fakeDeviceManager.FindLegoDevice(dev1.DeviceID).Returns(dev1);
            fakeDeviceManager.FindLegoDevice(dev2.DeviceID).Returns(dev2);
            
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            Assert.True(deviceAddressLookup.TryGetAddressForLEGODevice(dev2, out var address));
            Assert.True(address != -1);
            Assert.True(address != 0);
        }
        
        [Test]
        public void TryGetAddress_MaxNumberIsAddressedAndConnected_ReturnsFalse()
        {
            ILEGODevice deviceFactory(string deviceName)
            {
                var device = Substitute.For<ILEGODevice>();
                device.DeviceID.Returns(deviceName);
                device.State.Returns(DeviceState.InterrogationFinished);
                fakeDeviceManager.FindLegoDevice(device.DeviceID).Returns(device);
                return device;
            }

            var devices = Enumerable.Range(1, 4)
                                    .ToDictionary(i => i - 1, i => deviceFactory("dev" + i));

            deviceAddressLookup.SetLegoDeviceOrder(devices);

            var dev5 = deviceFactory("dev5");
            Assert.False(deviceAddressLookup.TryGetAddressForLEGODevice(dev5, out _));
        }
        
        [Test]
        public void TryGetAddress_MaxNumberIsAddressedSingleDisconnected_ReturnsDisconnectedAddress()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            dev1.State.Returns(DeviceState.InterrogationFinished);
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            dev2.State.Returns(DeviceState.InterrogationFinished);
            var dev3 = Substitute.For<ILEGODevice>();
            dev3.DeviceID.Returns("dev3");
            dev3.State.Returns(DeviceState.DisconnectedNotAdvertising);
            var dev4 = Substitute.For<ILEGODevice>();
            dev4.DeviceID.Returns("dev4");
            dev4.State.Returns(DeviceState.InterrogationFinished);
            
            var dev5 = Substitute.For<ILEGODevice>();
            dev5.DeviceID.Returns("dev5");
            
            fakeDeviceManager.FindLegoDevice(dev1.DeviceID).Returns(dev1);
            fakeDeviceManager.FindLegoDevice(dev2.DeviceID).Returns(dev2);
            fakeDeviceManager.FindLegoDevice(dev3.DeviceID).Returns(dev3);
            fakeDeviceManager.FindLegoDevice(dev4.DeviceID).Returns(dev4);
            fakeDeviceManager.FindLegoDevice(dev5.DeviceID).Returns(dev5);
            
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2},
                {2, dev3},
                {3, dev4}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            Assert.True(deviceAddressLookup.TryGetAddressForLEGODevice(dev5, out var address));
            Assert.True(address == 2);
        }
        
        [Test]
        public void GetLEGODeviceForAddress_ConnectedDevicesExists_ReturnsDeviceForAddress()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            dev1.State.Returns(DeviceState.DisconnectedNotAdvertising);
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            
            //Make sure deviceManager returns the actual devices, because DeviceAddressLookup depends on it
            fakeDeviceManager.FindLegoDevice(dev1.DeviceID).Returns(dev1);
            fakeDeviceManager.FindLegoDevice(dev2.DeviceID).Returns(dev2);
            
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            Assert.True(deviceAddressLookup.GetLEGODeviceForAddress(0) == dev1);
            Assert.True(deviceAddressLookup.GetLEGODeviceForAddress(1) == dev2);
        }
        
        [Test]
        public void GetLEGODeviceForAddress_DeviceDoesNotExist_ReturnsNull()
        {
            var dev1 = Substitute.For<ILEGODevice>();
            dev1.DeviceID.Returns("dev1");
            dev1.State.Returns(DeviceState.DisconnectedNotAdvertising);
            var dev2 = Substitute.For<ILEGODevice>();
            dev2.DeviceID.Returns("dev2");
            var devs = new Dictionary<int, ILEGODevice>()
            {
                {0, dev1},
                {1, dev2}
            };
            deviceAddressLookup.SetLegoDeviceOrder(devs);

            Assert.True(deviceAddressLookup.GetLEGODeviceForAddress(2) == null);
        }

        [Test]
        public void GetLEGODeviceForAddress_GivenNonExistingAddress_DoesNotThrowAndReturnsNull()
        {
            ILEGODevice result = null;
            Assert.DoesNotThrow(() =>
            {result = deviceAddressLookup.GetLEGODeviceForAddress(-10);});
            Assert.IsNull(result);
        }
    }
}