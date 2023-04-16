// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LEGOWirelessSDK
{
    public class TiltSensorThreeAxis : ServiceBase
    {
        public TiltSensorThreeAxisMode Mode { get => mode; set { if (mode != value) { mode = value; UpdateInputFormat(); } } }


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
            get => tilt; // LETiltSensorThreeAxisOrientation.
            private set
            {
                tilt = value;
                TiltChanged.Invoke(tilt);
            }
        }

        public bool Shake
        {
            get => shake;
            private set
            {
                shake = value;
                ShakeChanged.Invoke(shake);
            }

        }

        public Vector3 Acceleration
        {
            get => acceleration; // Gravity - 0 to 1.
            private set
            {
                acceleration = value;
                AccelerationChanged.Invoke(acceleration);
            }
        }

        public UnityEvent<Vector3> AngleChanged;
        public UnityEvent<int> TiltChanged;
        public UnityEvent<bool> ShakeChanged;
        public UnityEvent<Vector3> AccelerationChanged;

        #region internals
        public enum TiltSensorThreeAxisMode
        {
            Angle = 0,
            //Tilt = 1,
            [InspectorName("Tilt")]
            Orientation = 2,
            [InspectorName("Shake")]
            Impact = 3,
            Acceleration = 4
        }

        private LEGOTiltSensorThreeAxis sensor;
        [SerializeField] private TiltSensorThreeAxisMode mode = TiltSensorThreeAxisMode.Angle;
        private Vector3 angle;
        private int tilt;
        private bool shake;
        private float timeSinceLastShake;
        private Vector3 acceleration;

        const float shakeResetThreshold = 0.1f;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeInternalTiltSensorThreeAxis) as LEGOTiltSensorThreeAxis;
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
            if (shake)
            {
                timeSinceLastShake += Time.deltaTime;

                if (timeSinceLastShake > shakeResetThreshold)
                {
                    Shake = false;
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
            if (newValue.Mode == (int)TiltSensorThreeAxisMode.Angle)
            {
                Angle = new Vector3(newValue.SIValues[1], 0f, newValue.SIValues[0]);
            }
            else if (newValue.Mode == (int)TiltSensorThreeAxisMode.Orientation)
            {
                var tilt = (int)newValue.RawValues[0];
                // Flip left/right + forward/backward to be consistent with other sensor values.
                if (tilt == 3 || tilt == 1)
                {
                    tilt += 1;
                }
                else if (tilt == 4 || tilt == 2)
                {
                    tilt -= 1;
                }
                Tilt = tilt;
            }
            else if (newValue.Mode == (int)TiltSensorThreeAxisMode.Impact)
            {
                Shake = true;
                timeSinceLastShake = 0f;
            }
            else if (newValue.Mode == (int)TiltSensorThreeAxisMode.Acceleration)
            {
                // Full gravity is 65, so normalize by that.
                Acceleration = new Vector3(newValue.SIValues[0], -newValue.SIValues[2], newValue.SIValues[1]) / 65f;
            }
        }
        #endregion
    }
}