using System;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.Serialization;

namespace Core.Scripts
{
    public interface IQuickTimeSpinWheelEvent : IQuickTimeEvent
    {
    
    }

    public interface IQuickTimeSpinWheelDependencies : IQuickTimeEventDependencies
    {
        ISpinWheelInDirectionController SpinWheelController { get; }
        SpinDirections DirectionsRequired { get; }
    }

    public interface ISpinDirectionPayload
    {
        SpinDirection Direction { get; }
    }

    public class SpinDirectionPayload : ISpinDirectionPayload
    {
        public SpinDirectionPayload(SpinDirection direction)
        {
            Direction = direction;
        }

        public SpinDirection Direction { get; }
    }
    public interface ISpinWheelInDirectionController
    {
        void SubscribeToSpinDirectionEvent(Action<ISpinDirectionPayload> actionToSubscribe);
        void ClearSubscriptions();
        void StopSpinning();
        void SpinInDirection(SpinDirection direction);
    }

    public interface IQuickTimeSpinWheelPayload : IQuickTimeEventPayload
    {
    }

    public class QuickTimeSpinWheelPayload : IQuickTimeSpinWheelPayload
    {
        public QuickTimeSpinWheelPayload(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
    public class SpinDirections
    {
        public SpinDirections(List<SpinDirectionData> directionsRequired)
        {
            DirectionsRequired = directionsRequired;
        }

        public List<SpinDirectionData> DirectionsRequired { get; }

        public bool IsComplete()
        {
            foreach (var directionData in DirectionsRequired)
            {
                if (!directionData.Completed)
                    return false;
            }
            return true;
        }

        public void MarkComplete(int idx)
        {
            var point = DirectionsRequired[idx];
            point.Completed = true;
        }
    }
    public class SpinDirectionData
    {
        public SpinDirectionData(SpinDirection targetDirection, int timeToSpin)
        {
            TargetDirection = targetDirection;
            TimeToSpin = timeToSpin;
        }

        public SpinDirection TargetDirection { get; }
        public int TimeToSpin { get; }
        public bool Completed { get; set; }
    }
    public enum SpinDirection
    {
        None,
        Forward,
        Backward
    }
    public class QuickTimeSpinWheelEvent : QuickTimeEvent, IQuickTimeSpinWheelEvent
    {
        [SerializeField] private Sprite rotateForwards;
        [SerializeField] private Sprite rotateBackwards;
        private IQuickTimeSpinWheelDependencies dependencies;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is IQuickTimeSpinWheelDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.SpinWheelController.SubscribeToSpinDirectionEvent(WheelSpun);
                idx = 0;
            }
            else throw new ArgumentException("Wrong dependency type given.");
        }

        private int idx = 0;
        public override void PlayEvent()
        {
            DisplayNextSpin(dependencies.DirectionsRequired.DirectionsRequired[idx]);
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private void DisplayNextSpin(SpinDirectionData directionData)
        {
            dependencies.Shower.Show(directionData.TargetDirection == SpinDirection.Backward ? rotateBackwards : rotateForwards);
            dependencies.SpinWheelController.StopSpinning();
            dependencies.SpinWheelController.SpinInDirection(directionData.TargetDirection == SpinDirection.Backward ? SpinDirection.Backward : SpinDirection.Forward);
            Debug.Log($"Please Spin the wheel: {directionData.TargetDirection}");
        }

        private void WheelSpun(ISpinDirectionPayload spinDirectionPayload)
        {
            // dependencies.SpinWheelController.StopSpinning();
            // dependencies.SpinWheelController.SpinInDirection(spinDirectionPayload.Direction);
            var directionData = dependencies.DirectionsRequired.DirectionsRequired[idx];
            if (directionData.TargetDirection == spinDirectionPayload.Direction)
            {
                dependencies.DirectionsRequired.MarkComplete(idx);
                idx++;
                if (dependencies.DirectionsRequired.IsComplete())
                {
                    CompleteEvent(true);
                }
                else
                {
                    directionData = dependencies.DirectionsRequired.DirectionsRequired[idx];
                    DisplayNextSpin(directionData);
                }
            }
        }

        private bool _success = false;
        private void CompleteEvent(bool success)
        {
            _success = success;
            if(success)
                dependencies.WinSoundEmitter.Play();
            else 
                dependencies.LossSoundEmitter.Play();

            Invoke("DelayedCompleteEvent", 2f);
        }

        private void DelayedCompleteEvent()
        {
            dependencies.CompleteEvent(new QuickTimeSpinWheelPayload(_success));
            dependencies.Shower.Hide();
            dependencies.SpinWheelController.ClearSubscriptions();
            dependencies.SpinWheelController.StopSpinning();
        }
    }
}