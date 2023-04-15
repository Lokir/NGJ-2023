// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LEGOWirelessSDK
{
    public class MotionSensor : ServiceBase
    {
        public MotionSensorMode Mode
        {
            get => mode;
            set
            {
                if (mode != value)
                {
                    mode = value;
                    UpdateInputFormat();
                }
            }
        }

        public int Distance
        {
            get => distance; // 0 to 10.
            private set
            {
                distance = value;
                DistanceChanged.Invoke(distance);
            }
        }

        public bool Detect
        {
            get => detect;
            private set
            {
                detect = value;
                DetectChanged.Invoke(detect);
            }
        }

        public UnityEvent<int> DistanceChanged;
        public UnityEvent<bool> DetectChanged;

        #region internals
        public enum MotionSensorMode
        {
            Distance = 0,
            Detect = 1
        }

        private LEGOMotionSensor sensor;
        [SerializeField] private MotionSensorMode mode = MotionSensorMode.Distance;
        private int distance;
        private bool detect;
        private float timeSinceLastDetect;

        const float detectResetThreshold = 0.1f;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeMotionSensor) as LEGOMotionSensor;
            if (sensor == null)
            {
                Debug.LogWarning(name + " service not found");
                return false;
            }
            services.Remove(sensor);
            UpdateInputFormat();
            sensor.RegisterDelegate(this);
            IsConnected = true;
            Debug.Log(name + " connected");
            return true;
        }

        private void UpdateInputFormat()
        {
            sensor?.UpdateInputFormat(new LEGOInputFormat(sensor.ConnectInfo.PortID, sensor.ioType, (int)mode, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
        }

        private void Update()
        {
            if (detect)
            {
                timeSinceLastDetect += Time.deltaTime;

                if (timeSinceLastDetect > detectResetThreshold)
                {
                    Detect = false;
                }
            }
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
            if (newValue.Mode == (int)MotionSensorMode.Distance)
            {
                Distance = Mathf.RoundToInt(newValue.RawValues[0]);
            }
            else if (newValue.Mode == (int)MotionSensorMode.Detect)
            {
                Detect = true;
                timeSinceLastDetect = 0f;
            }
        }
        #endregion
    }
}