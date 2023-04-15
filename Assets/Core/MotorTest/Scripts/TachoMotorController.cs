using System;
using System.Collections;
using System.Collections.Generic;
using LEGOWirelessSDK;
using UnityEngine;

namespace Core.MotorTest.Scripts
{
    public interface IWheelChangeEventPayload
    {
        int NewPosition { get; }
        bool Stopped { get; }
    }

    public class WheelChangeEventPayload: IWheelChangeEventPayload
    {
        public WheelChangeEventPayload(int newPosition, bool stopped)
        {
            NewPosition = newPosition;
            Stopped = stopped;
        }

        public int NewPosition { get; }
        public bool Stopped { get; }
    }
    public interface IVirtualMill
    {
        bool IsInitialized { get; }
        bool IsMoving { get; }
        void SubscribeToOnWheelChanged(Action<IWheelChangeEventPayload> changeEvent);
    }

    public interface ITachoMotorController : IVirtualMill
    {
        void WaitForInitialize(Action tachoMotorInitializedEvent);
    }

    [RequireComponent(typeof(TachoMotorWithAbsolutePosition))]
    public class TachoMotorController : MonoBehaviour, ITachoMotorController
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsMoving { get; private set; } = false;

        private Action newTachoMotorInitialized;
        public void WaitForInitialize(Action tachoMotorInitializedEvent)
        {
            newTachoMotorInitialized = tachoMotorInitializedEvent;
        }
        
        private TachoMotorWithAbsolutePosition motor;
        public void OnMotorInitialized()
        {
            IsInitialized = true;
            _newTargetPosition = (t) => { };
            motor = GetComponent<TachoMotorWithAbsolutePosition>();
            newTachoMotorInitialized.Invoke();
            StartDefaultBehaviour();
        }
        
        private void StartDefaultBehaviour()
        {
            if (!IsInitialized) return;
            ContinueToPosition();
        }
        
        private Action<IWheelChangeEventPayload> _newTargetPosition;
        public void SubscribeToOnWheelChanged(Action<IWheelChangeEventPayload> changeEvent)
        {
            _newTargetPosition += changeEvent;
        }

        public void OnMotorChangedPosition()
        {
            _newTargetPosition.Invoke(new WheelChangeEventPayload(motor.Position, false));
            ContinueToPosition();
        }

        private void ContinueToPosition()
        {
            motor.GoToPosition(motor.Position+100, false, 200);
        }
    }
}