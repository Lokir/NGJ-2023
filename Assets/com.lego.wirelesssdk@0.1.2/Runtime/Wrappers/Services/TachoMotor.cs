// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LEGOWirelessSDK
{
    public class TachoMotor : Motor
    {
        public TachoMotorMode Mode { get => mode; set { if (mode != value) { mode = value; UpdateInputFormat(); } } }

        public int Position
        {
            get => position; // Degrees.
            protected set
            {
                position = value;
                PositionChanged.Invoke(position);
            }
        }

        public int Speed
        {
            get => speed; // Unsure.
            protected set
            {
                speed = value;
                SpeedChanged.Invoke(speed);
            }
        }

        public int Power
        {
            get => power; // Unsure.
            protected set
            {
                power = value;
                PowerChanged.Invoke(power);
            }
        }

        public UnityEvent<int> PositionChanged;
        public UnityEvent<int> SpeedChanged;
        public UnityEvent<int> PowerChanged;

        public void GoToPosition(int pos, bool brake = false, int speed = 100)
        {
            if (tachoMotor == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            var cmd = new LEGOTachoMotorCommon.SetSpeedPositionCommand()
            {
                Position = pos,
                Speed = speed
            };
            cmd.SetEndState(brake ? MotorWithTachoEndState.Braking : MotorWithTachoEndState.Drifting);
            tachoMotor.SendCommand(cmd);
        }

        public void HoldPosition(int pos)
        {
            if (tachoMotor == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            var cmd = new LEGOTachoMotorCommon.SetSpeedPositionCommand()
            {
                Position = pos,
                Speed = 100
            };
            cmd.SetEndState(MotorWithTachoEndState.Holding);
            tachoMotor.SendCommand(cmd);
        }
        public void SetSpeed(int speed) {
            var cmd = new LEGOTachoMotorCommon.SetSpeedCommand() {
                Speed = speed
            };
            tachoMotor.SendCommand(cmd);
            motor.SendCommand(motor.SetPowerCommand(power));
        }

        public void SpinForTime(int time, int speed = 100, bool brake = false)
        {
            if (tachoMotor == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            var cmd = new LEGOTachoMotorCommon.SetSpeedMilliSecondsCommand()
            {
                MilliSeconds = time,
                Speed = speed
            };
            cmd.SetEndState(brake ? MotorWithTachoEndState.Braking : MotorWithTachoEndState.Drifting);
            tachoMotor.SendCommand(cmd);
        }

        public void Hold()
        {
            if (tachoMotor == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            var stopCmd = new LEGOTachoMotorCommon.HoldCommand(); // max speed?
            tachoMotor.SendCommand(stopCmd);
        }

        public void ResetPosition()
        {
            if (tachoMotor == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            var resetCmd = new LEGOTachoMotorCommon.PresetEncoderCommand()
            {
                Preset = 0
            };
            tachoMotor.SendCommand(resetCmd);
        }

        #region internals
        public enum TachoMotorMode { Power, Speed, Position, SpeedAndPosition }

        [Flags]
        new public enum MotorIOTypes
        {
            Clear = 0,
            MotorWithTacho = 1 << 3,
            InternalMotorWithTacho = 1 << 4,
            Any = ~0
        }

        new public enum MotorIOTypesWithoutClear // could be done with obsolete but it will say Clear (Obsolete)
        {
            Any = ~0,
            MotorWithTacho = 1 << 3,
            InternalMotorWithTacho = 1 << 4
        }

        protected ILEGOTachoMotor tachoMotor;
        [SerializeField] private TachoMotorMode mode = TachoMotorMode.Power;
        protected int position;
        protected int speed;
        protected int power;

        private bool MatchingService(ILEGOService service)
        {
            if (virtualConn != service.ConnectInfo.VirtualConnection || (port != -1 && port != service.ConnectInfo.PortID) || service.ServiceName == "Undefined")
            {
                return false;
            }
            MotorIOTypes t;
            if (!Enum.TryParse(service.ioType.ToString().Substring("LEIOType".Length), out t))
            {
                return false;
            }
            return (typeBitMask & (int)t) == (int)t;
        }

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            tachoMotor = services.FirstOrDefault(MatchingService) as ILEGOTachoMotor;
            base.motor = tachoMotor;
            if (tachoMotor == null)
            {
                Debug.LogWarning(name + " service not found");
                return false;
            }
            services.Remove(tachoMotor);
            UpdateInputFormat();
            tachoMotor.RegisterDelegate(this);
            IsConnected = true;
            Debug.Log(name + " connected");
            return true;
        }

        private void UpdateInputFormat()
        {
            if (tachoMotor == null)
            {
                return;
            }

            if (mode == TachoMotorMode.SpeedAndPosition)
            {
                tachoMotor.ResetCombinedModesConfiguration();
                tachoMotor.AddCombinedMode((int)MotorWithTachoMode.Speed, 1);
                tachoMotor.AddCombinedMode((int)MotorWithTachoMode.Position, 1);
                tachoMotor.ActivateCombinedModes();
                return;
            }

            int modeNo = tachoMotor.DefaultInputFormat.Mode;
            switch (mode)
            {
                case TachoMotorMode.Power: modeNo = tachoMotor.PowerModeNo; break;
                case TachoMotorMode.Speed: modeNo = tachoMotor.SpeedModeNo; break;
                case TachoMotorMode.Position: modeNo = tachoMotor.PositionModeNo; break;
            }
            tachoMotor.UpdateInputFormat(new LEGOInputFormat(tachoMotor.ConnectInfo.PortID, tachoMotor.ioType, modeNo, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
        }

        public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
        {
            if (newState == ServiceState.Disconnected)
            {
                tachoMotor = null;
            }
            base.DidChangeState(service, oldState, newState);
        }

        public override void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == tachoMotor.PositionModeNo)
            {
                Position = (int)newValue.RawValues[0];
            }
            else if (newValue.Mode == tachoMotor.SpeedModeNo)
            {
                Speed = (int)newValue.RawValues[0];
            }
            else if (newValue.Mode == tachoMotor.PowerModeNo)
            {
                Power = (int)newValue.RawValues[0];
            }
        }
        #endregion
    }
}