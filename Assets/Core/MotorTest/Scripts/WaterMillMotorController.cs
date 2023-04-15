using System;
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

    public class MotorRecordings
    {
        public int AverageMovement => GetAverageMovement();
        private int idx = 0;
        private int[] recordedMotorMovement = new [] {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };
        private int GetAverageMovement()
        {
            int sum = 0;
            foreach (var record in recordedMotorMovement)
            {
                sum += Mathf.Abs(record);
            }
            return Mathf.CeilToInt(sum / recordedMotorMovement.Length);
        }

        public void RecordEntry(int entry)
        {
            recordedMotorMovement[idx] = entry;
            IncrementEntry();
        }

        private void IncrementEntry()
        {
            idx++;
            if (idx > recordedMotorMovement.Length)
                idx = 0;
        }
    }

    //[RequireComponent(typeof(TachoMotorWithAbsolutePosition))]
    public class WaterMillMotorController : MonoBehaviour, IWaterMillMotorController
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsMoving { get; private set; } = false;


        private MotorRecordings motorRecordings;
        private Action newTachoMotorInitialized;
        public void WaitForInitialize(Action tachoMotorInitializedEvent)
        {
            newTachoMotorInitialized = tachoMotorInitializedEvent;
        }
        
        // private TachoMotorWithAbsolutePosition motor;
        private TachoMotor motor;
        public void OnMotorInitialized()
        {
            IsInitialized = true;
            _newTargetPosition = (t) => { };
            motorRecordings = new MotorRecordings();
            // motor = GetComponent<TachoMotorWithAbsolutePosition>();
            motor = GetComponent<TachoMotor>();
            motor.Drift();
            newTachoMotorInitialized.Invoke();
            StartDefaultBehaviour();
        }
        
        private void StartDefaultBehaviour()
        {
            if (!IsInitialized) return;
            AssignContinousMotorState(true, 30);
        }
        
        private Action<IWheelChangeEventPayload> _newTargetPosition;
        public void SubscribeToOnWheelChanged(Action<IWheelChangeEventPayload> wheelMotionChangedEvent)
        {
            _newTargetPosition += wheelMotionChangedEvent;
        }

        [SerializeField] private int lastPosition = 0;
        public void OnMotorChangedPosition()
        {
            var deltaPosition = motor.Position - lastPosition;
            if (IsMoving)
                WhenMotorRunningAndPositionChanged(deltaPosition);
            else
                WhenMotorStoppedAndPositionChanged(deltaPosition);
            motorRecordings.RecordEntry(deltaPosition);
        }
        
        private void WhenMotorRunningAndPositionChanged(int deltaPosition)
        {
            var isStopped = IsMotorForciblyStopped(deltaPosition);
            AssignContinousMotorState(!isStopped, deltaPosition);
        }
        
        private void WhenMotorStoppedAndPositionChanged(int deltaPosition)
        {
            var shouldMove = ShouldMotorRestart(deltaPosition);
            AssignContinousMotorState(shouldMove, deltaPosition);
        }

        private void AssignContinousMotorState(bool shouldMove, int deltaPosition)
        {
            if (shouldMove)
            {
                Debug.Log("Started");
                ContinueToPosition(deltaPosition <= 0);
                IsMoving = true;
            }
            else
            {
                Debug.Log("Stopped");
                lastPosition = 0;
                motor.SetSpeed(0);
                IsMoving = false;
            }
            _newTargetPosition.Invoke(new WheelChangeEventPayload(motor.Position, IsMoving));
        }

        private bool IsMotorForciblyStopped(int deltaPosition)
        {
            var absoluteDeltaPosition = Mathf.Abs(deltaPosition);
            Debug.Log($"Absolute Delta: {absoluteDeltaPosition} : AverageMovement {motorRecordings.AverageMovement}");
            if (motorRecordings.AverageMovement <  absoluteDeltaPosition)
                return true;
            return false;
        }

        private bool ShouldMotorRestart(int deltaPosition)
        {
            var absoluteDeltaPosition = Mathf.Abs(deltaPosition);
            if (absoluteDeltaPosition > 0)
                return true;
            return false;
        }

        private void ContinueToPosition(bool forwardDirection)
        {
            lastPosition = motor.Position;
            var delta = 25;
            delta *= forwardDirection ? -1 : 1;
            var nextPosition = motor.Position + delta;
            //Debug.Log($"Going to position: {nextPosition}");
            motor.GoToPosition(nextPosition, false, 12);
        }
    }
}