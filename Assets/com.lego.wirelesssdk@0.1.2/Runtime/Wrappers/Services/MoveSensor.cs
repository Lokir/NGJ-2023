// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LEGOWirelessSDK
{
    public class MoveSensor : ServiceBase
    {
        public int Speed
        {
            get => speed; // Unsure.
            private set
            {
                speed = value;
                SpeedChanged.Invoke(speed);
            }
        }

        public UnityEvent<int> SpeedChanged;

        #region internals
        private LEGOMoveSensor sensor;
        private int speed;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeMoveSensor) as LEGOMoveSensor;
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
            Speed = (int)newValue.RawValues[0];
        }
        #endregion
    }
}