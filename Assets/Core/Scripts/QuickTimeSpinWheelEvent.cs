using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public int Idx = 0;
        public List<SpinDirectionData> DirectionsRequired { get; }

        public bool IsComplete()
        {
            foreach (var directionData in DirectionsRequired)
            {
                if (!directionData.Completed)
                    return false;
            }
            return false;
        }

        public void MarkComplete(int idx)
        {
            var point = DirectionsRequired[idx];
            point.Completed = true;
            Idx++;
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
        Backward,
        Either
    }
    public class QuickTimeSpinWheelEvent : QuickTimeEvent, IQuickTimeSpinWheelEvent
    {
        private IQuickTimeSpinWheelDependencies dependencies;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is IQuickTimeSpinWheelDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.SpinWheelController.SubscribeToSpinDirectionEvent(WheelSpun);
            }
            else throw new ArgumentException("Wrong dependency type given.");
        }

        public override void PlayEvent()
        {
            var idx = dependencies.DirectionsRequired.Idx;
            DisplayNextSpin(dependencies.DirectionsRequired.DirectionsRequired[idx]);
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private void DisplayNextSpin(SpinDirectionData directionData)
        {
            dependencies.SpinWheelController.StopSpinning();
            Debug.Log($"Spin direction: {directionData.TargetDirection}");
        }

        private void WheelSpun(ISpinDirectionPayload spinDirectionPayload)
        {
            var idx = dependencies.DirectionsRequired.Idx;
            var directionData = dependencies.DirectionsRequired.DirectionsRequired[idx];
            if (directionData.TargetDirection == spinDirectionPayload.Direction ||
                directionData.TargetDirection == SpinDirection.Either)
            {
                dependencies.DirectionsRequired.MarkComplete(idx);
                if (dependencies.DirectionsRequired.IsComplete())
                {
                    CompleteEvent(true);
                }
                else
                {
                    idx = dependencies.DirectionsRequired.Idx;
                    directionData = dependencies.DirectionsRequired.DirectionsRequired[idx];
                    DisplayNextSpin(directionData);
                }
            }
        }

        private void CompleteEvent(bool success)
        {
            dependencies.CompleteEvent(new QuickTimeSpinWheelPayload(success));
            dependencies.SpinWheelController.ClearSubscriptions();
        }
    }
}