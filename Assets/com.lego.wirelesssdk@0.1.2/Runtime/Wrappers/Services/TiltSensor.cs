// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LEGOWirelessSDK
{
    public class TiltSensor : ServiceBase
    {
        public TiltSensorMode Mode { get => mode; set { if (mode != value) { mode = value; UpdateInputFormat(); } } }


        public Vector3 Angle
        {
            get => angle; // Euler angles.
            private set
            {
                angle = value;
                AngleChanged.Invoke(angle);
            }
        }

        public int Tilt
        {
            get => tilt; // LEGOTiltSensor.LETiltSensorDirection.
            private set
            {
                tilt = value;
                TiltChanged.Invoke(tilt);
            }
        }

        public (bool X, bool Y, bool Z) Shake
        {
            get => shake;
            private set
            {
                shake = value;
                ShakeChanged.Invoke(shake);
            }

        }

        public UnityEvent<Vector3> AngleChanged;
        public UnityEvent<int> TiltChanged;
        public UnityEvent<(bool X, bool Y, bool Z)> ShakeChanged;

        #region internals
        public enum TiltSensorMode
        {
            Angle = 0,
            Tilt = 1,
            [InspectorName("Shake")]
            Crash = 2
        }

        private LEGOTiltSensor sensor;
        [SerializeField] private TiltSensorMode mode = TiltSensorMode.Angle;
        private Vector3 angle;
        private int tilt;
        private (bool X, bool Y, bool Z) shake;
        private (float X, float Y, float Z) timeSinceLastShake;

        const float shakeResetThreshold = 0.1f;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTiltSensor) as LEGOTiltSensor;
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
            var shakeChanged = false;

            if (shake.X)
            {
                timeSinceLastShake.X += Time.deltaTime;

                if (timeSinceLastShake.X > shakeResetThreshold)
                {
                    shake.X = false;
                    shakeChanged = true;
                }
            }

            if (shake.Y)
            {
                timeSinceLastShake.Y += Time.deltaTime;

                if (timeSinceLastShake.Y > shakeResetThreshold)
                {
                    shake.Y = false;
                    shakeChanged = true;
                }
            }

            if (shake.Z)
            {
                timeSinceLastShake.Z += Time.deltaTime;

                if (timeSinceLastShake.Z > shakeResetThreshold)
                {
                    shake.Z = false;
                    shakeChanged = true;
                }
            }

            if (shakeChanged)
            {
                Shake = shake;
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
            if (newValue.Mode == (int)TiltSensorMode.Angle)
            {
                Angle = new Vector3(newValue.SIValues[1], 0f, -newValue.SIValues[0]);
            }
            else if (newValue.Mode == (int)TiltSensorMode.Tilt)
            {
                Tilt = (int)newValue.RawValues[0];
            }
            else if (newValue.Mode == (int)TiltSensorMode.Crash)
            {
                if (newValue.RawValues[0] > 0f)
                {
                    shake.X = true;
                    timeSinceLastShake.X = 0f;
                }

                if (newValue.RawValues[1] > 0f)
                {
                    shake.Y = true;
                    timeSinceLastShake.Y = 0f;
                }

                if (newValue.RawValues[2] > 0f)
                {
                    shake.Z = true;
                    timeSinceLastShake.Z = 0f;
                }

                Shake = shake;

                // Since the sensor stops sending updates once a shake counter reaches 100, we reset the shake counters on each update.
                sensor.SendResetStateRequest();

            }
        }
        #endregion
    }
}