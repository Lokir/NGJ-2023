// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LEGOWirelessSDK
{
    public class AccelerationSensor : ServiceBase
    {
        public Vector3 Acceleration
        {
            get => acceleration; // Gravity - 0 to 10, where 1 is normal gravity.
            private set
            {
                acceleration = value;
                AccelerationChanged.Invoke(acceleration);
            }
        }

        public UnityEvent<Vector3> AccelerationChanged;

        #region internals
        private LEGOTechnic3AxisAccelerationSensor sensor;
        private bool accelerometerFlipped;
        private Vector3 acceleration;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTechnic3AxisAccelerometer) as LEGOTechnic3AxisAccelerationSensor;
            if (sensor == null)
            {
                Debug.LogWarning(name + " service not found");
                return false;
            }
            services.Remove(sensor);
            sensor.UpdateInputFormat(new LEGOInputFormat(sensor.ConnectInfo.PortID, sensor.ioType, 0, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
            sensor.RegisterDelegate(this);
            IsConnected = true;
            Debug.Log(name + " connected");

            // The accelerometer values are flipped on Spike Essential.
            accelerometerFlipped = sensor.Device.SystemType == DeviceSystemType.LEGOTechnic1 && sensor.Device.SystemDeviceNumber == 3;

            return true;
        }

        private void OnDestroy()
        {
            sensor?.UnregisterDelegate(this);
        }

        public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
        {
            if (newState == ServiceState.Disconnected)
            {
                Debug.LogWarning(name + " disconnected");
                sensor.UnregisterDelegate(this);
                sensor = null;
                IsConnected = false;
            }
        }

        public override void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue)
        {
            // The SI is mG, so dividing by 1000 gives G.
            if (accelerometerFlipped)
            {
                Acceleration = new Vector3(newValue.SIValues[0], -newValue.SIValues[2], newValue.SIValues[1]) / 1000f;
            }
            else
            {
                Acceleration = new Vector3(newValue.SIValues[1], -newValue.SIValues[2], -newValue.SIValues[0]) / 1000f;
            }
        }
        #endregion
    }
}