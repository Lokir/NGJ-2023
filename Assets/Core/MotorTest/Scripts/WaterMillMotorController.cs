using System;
using System.Collections.Generic;
using Core.Scripts;
using Helpers;
using LEGODeviceUnitySDK;
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

    public interface IWaterMillMotorController : IVirtualMill, ISpinWheelInDirectionController
    {
        void WaitForInitialize(Action tachoMotorInitializedEvent);
    }

    public class WaterMillMotorController : MonoBehaviour, IWaterMillMotorController
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsMoving { get; private set; } = false;

        private Action<ISpinDirectionPayload> actionToSubscribe;
        private Action tachoMotorInitialized;
        public void WaitForInitialize(Action tachoMotorInitializedEvent)
        {
            tachoMotorInitialized = tachoMotorInitializedEvent;
            actionToSubscribe = (t) => { };
        }

        private TachoMotor motor;
        public void OnMotorInitialized()
        {
            IsInitialized = true;
            _newTargetPosition = (t) => { };
            motor = GetComponent<TachoMotor>();
            motor.Drift();
            tachoMotorInitialized.Invoke();
        }
        
        private void StartDefaultBehaviour()
        {
            // if (!IsInitialized) return;
            // AssignContinousMotorState(true);
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

        private SpinDirection currentSpinDirection;
        public void OnSpeedChanged()
        {
            var direction = SpinDirection.None;
            if (motor.Speed > 0) direction = SpinDirection.Backward;
            else if (motor.Speed < 0) direction = SpinDirection.Forward;
            
            if (!MotorIsForciblyStopped() && IsMoving)
            {
                StopMotor();
            }
            else if(!IsMoving)
            {
                AssignContinousMotorState(direction);
            }
            else if (IsMoving && IsNotSameDirection(direction))
            {
                Debug.Log($"Changing Direction: {direction}");
                currentSpinDirection = direction;
                actionToSubscribe.Invoke(new SpinDirectionPayload(direction));
                AssignContinousMotorState(currentSpinDirection);
            }
        }

        private bool IsNotSameDirection(SpinDirection newSpinDirection)
        {
            if (currentSpinDirection == newSpinDirection) return false;
            return true;
        }

        private bool MotorIsForciblyStopped()
        {
            if (Mathf.Abs(motor.Speed) < 3f)
                return false;
            return true;
        }
        
        private void StopMotor()
        {
            motor.SetSpeed(0);
            motor.SetPower(0);
            IsMoving = false;
        }

        private void AssignContinousMotorState(SpinDirection direction)
        {
            IsMoving = true;
            SetSpeed(direction);
        }
        
        private void SetSpeed(SpinDirection direction)
        {
            var speed = 12;
            if (direction == SpinDirection.Backward)
                speed *= -1;
            if (speed != motor.Speed)
            {
                motor.SetSpeed(speed);
            }
        }
        
        public void SubscribeToSpinDirectionEvent(Action<ISpinDirectionPayload> actionToSubscribe)
        {
            this.actionToSubscribe += actionToSubscribe;
        }

        public void ClearSubscriptions()
        {
            this.actionToSubscribe = (t) => { };
        }

        public void StopSpinning()
        {
            StopMotor();
        }

        public void SpinInDirection(SpinDirection direction)
        {
            currentSpinDirection = direction;
            AssignContinousMotorState(direction);
            Debug.Log("Spin In Direction");
        }
    }
}