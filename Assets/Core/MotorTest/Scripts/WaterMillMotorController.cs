using System;
using System.Collections.Generic;
using Helpers;
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
        void SubscribeToOnWheelChanged(Action<IWheelChangeEventPayload> wheelMotionChangedEvent);
    }

    public interface IWaterMillMotorController : IVirtualMill
    {
        void WaitForInitialize(Action tachoMotorInitializedEvent);
    }

    public class WaterMillMotorController : MonoBehaviour, IWaterMillMotorController
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsMoving { get; private set; } = false;

        private Action newTachoMotorInitialized;
        public void WaitForInitialize(Action tachoMotorInitializedEvent)
        {
            newTachoMotorInitialized = tachoMotorInitializedEvent;
        }
        
        private TachoMotor motor;
        public void OnMotorInitialized()
        {
            IsInitialized = true;
            _newTargetPosition = (t) => { };
            motor = GetComponent<TachoMotor>();
            motor.Drift();
            newTachoMotorInitialized.Invoke();
            StartDefaultBehaviour();
        }
        
        private void StartDefaultBehaviour()
        {
            if (!IsInitialized) return;
            AssignContinousMotorState(true);
        }
        
        private Action<IWheelChangeEventPayload> _newTargetPosition;
        public void SubscribeToOnWheelChanged(Action<IWheelChangeEventPayload> wheelMotionChangedEvent)
        {
            _newTargetPosition += wheelMotionChangedEvent;
        }

        public void OnPositionChanged()
        {
            if(motor.Speed != 0)
                _newTargetPosition.Invoke(new WheelChangeEventPayload(motor.Position, IsMoving));
        }


        public void Update()
        {
            if (!IsInitialized) return;
            Debug.Log(motor.Speed);
            
            if (!MotorIsForciblyStopped() && IsMoving)
            {
                StopMotor();
            }
            // else if (IsMoving)
            //     AssignContinousMovement();
        }

        public void OnSpeedChanged()
        {
            if(!IsMoving)
            {
                AssignContinousMovement();
            }
        }

        private bool MotorIsForciblyStopped()
        {
            if (Mathf.Abs(motor.Speed) < 2f)
                return false;
            return true;
        }

        private void AssignContinousMovement()
        {
            AssignContinousMotorState(motor.Speed < 0);
        }

        private void StopMotor()
        {
            Debug.Log("Stopped");
            motor.SetSpeed(0);
            IsMoving = false;
        }

        private void AssignContinousMotorState(bool forwardDirection)
        {
            IsMoving = true;
            SetSpeed(forwardDirection);
        }
        
        private void SetSpeed(bool forwardDirection)
        {
            var speed = 12;
            speed *= forwardDirection ? -1 : 1;
            if(speed != motor.Speed)
                motor.SetSpeed(speed);
        }
    }
}