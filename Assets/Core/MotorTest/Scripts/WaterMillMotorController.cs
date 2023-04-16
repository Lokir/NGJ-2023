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

        private ITimer timer;
        private TachoMotor motor;
        public void OnMotorInitialized()
        {
            IsInitialized = true;
            _newTargetPosition = (t) => { };
            motor = GetComponent<TachoMotor>();
            timer = gameObject.AddComponent<TimerFixedUpdateLoop>();
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

        private SpinDirection currentSpinDirection = SpinDirection.None;
        public void OnSpeedChanged()
        {
            var direction = GetCurrentDirectionBasedOnMovement();

            if (IsMoving && MotorIsForciblyStopped() && canStopAutomatically)
            {
                direction = SpinDirection.None;
            }
            Debug.Log(motor.Speed);
            if (IsNotSameDirection(direction))
            {
                Debug.Log($"Changing Direction: {direction}");
                currentSpinDirection = direction;
                actionToSubscribe.Invoke(new SpinDirectionPayload(direction));
            }
        }

        private SpinDirection GetCurrentDirectionBasedOnMovement()
        {
            if (!IsMoving) return SpinDirection.None;
            if (motor.Speed > 0) return SpinDirection.Forward;
            if (motor.Speed < 0)return SpinDirection.Backward;
            return SpinDirection.None;
        }

        private bool IsNotSameDirection(SpinDirection newSpinDirection)
        {
            if (currentSpinDirection == newSpinDirection) return false;
            return true;
        }

        private bool MotorIsForciblyStopped()
        {
            if (Mathf.Abs(motor.Speed) < 3f)
                return true;
            return false;
        }
        
        private void StopMotor()
        {
            currentSpinDirection = SpinDirection.None;
            motor.SetSpeed(0);
            motor.SetPower(0);
            IsMoving = false;
        }

        private void AssignContinousMotorState(SpinDirection direction)
        {
            if (direction == SpinDirection.None)
                throw new ArgumentException("Don't call this method with direction None");
            IsMoving = true;
            PauseMotorStop();
            SetSpeed(direction);
        }

        private bool canStopAutomatically = true;
        private void PauseMotorStop()
        {
            timer.StartTimer(0.3f, null, UnpauseMotorStop);
            canStopAutomatically = false;
        }

        public void UnpauseMotorStop()
        {
            canStopAutomatically = true;
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
            if (direction == SpinDirection.None)
            {
                StopMotor();
            }
            else
            {
                AssignContinousMotorState(direction);
            }
            Debug.Log("Spin In Direction");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("Backwards!");
                SpinInDirection(SpinDirection.Backward);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("Forwards!");
                SpinInDirection(SpinDirection.Forward);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Forwards!");
                SpinInDirection(SpinDirection.None);
            }
        }
    }
}