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
    public class TachoMotorWithAbsolutePosition : TachoMotor
    {
        new public TachoMotorMode Mode { get => modeWithAbsolute; set { if (modeWithAbsolute != value) { modeWithAbsolute = value; UpdateInputFormat(); } } }

        #region internals
        [Flags]
        new public enum MotorIOTypes
        {
            Clear = 0,
            TechnicAzureAngularMotorS = 1 << 5,
            TechnicAzureAngularMotorM = 1 << 6,
            TechnicAzureAngularMotorL = 1 << 7,
            TechnicGreyAngularMotorM = 1 << 8,
            TechnicGreyAngularMotorL = 1 << 9,
            TechnicMotorL = 1 << 10,
            TechnicMotorXL = 1 << 11,
            Any = ~0
        }

        new public enum MotorIOTypesWithoutClear // could be done with obsolete but it will say Clear (Obsolete)
        {
            Any = ~0,
            TechnicAzureAngularMotorS = 1 << 5,
            TechnicAzureAngularMotorM = 1 << 6,
            TechnicAzureAngularMotorL = 1 << 7,
            TechnicGreyAngularMotorM = 1 << 8,
            TechnicGreyAngularMotorL = 1 << 9,
            TechnicMotorL = 1 << 10,
            TechnicMotorXL = 1 << 11
        }

        new public enum TachoMotorMode { Power, Speed, Position, SpeedAndPosition, AbsolutePosition }
        [SerializeField] private TachoMotorMode modeWithAbsolute = TachoMotorMode.Power;

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

            if (modeWithAbsolute == TachoMotorMode.SpeedAndPosition)
            {
                tachoMotor.ResetCombinedModesConfiguration();
                tachoMotor.AddCombinedMode((int)MotorWithTachoMode.Speed, 1);
                tachoMotor.AddCombinedMode((int)MotorWithTachoMode.Position, 1);
                tachoMotor.ActivateCombinedModes();
                return;
            }

            int modeNo = tachoMotor.DefaultInputFormat.Mode;
            switch (modeWithAbsolute)
            {
                case TachoMotorMode.Power: modeNo = tachoMotor.PowerModeNo; break;
                case TachoMotorMode.Speed: modeNo = tachoMotor.SpeedModeNo; break;
                case TachoMotorMode.Position: modeNo = tachoMotor.PositionModeNo; break;
                case TachoMotorMode.AbsolutePosition: modeNo = tachoMotor.AbsolutePositionModeNo; break;
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
            else if (newValue.Mode == tachoMotor.AbsolutePositionModeNo)
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