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

        List<SpinDirectionData> DirectionsRequired { get; }

        public bool IsComplete()
        {
            foreach (var directionData in DirectionsRequired)
            {
                if (!directionData.Completed)
                    return false;
            }
            return false;
        }

        public SpinDirectionData GetNextInLine()
        {
            return DirectionsRequired.First(x => x.Completed == false);
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
            DisplayNextSpin(dependencies.DirectionsRequired.GetNextInLine());
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private void DisplayNextSpin(SpinDirectionData directionData)
        {
            Debug.Log($"Spin direction: {directionData.TargetDirection}");
        }

        private void WheelSpun(ISpinDirectionPayload spinDirectionPayload)
        {
            var directionData = dependencies.DirectionsRequired.GetNextInLine();
            if(directionData.TargetDirection == spinDirectionPayload.Direction || directionData.TargetDirection == SpinDirection.Either)
                directionData.Completed = true;
            if (dependencies.DirectionsRequired.IsComplete())
            {
                CompleteEvent(true);
            }
            else
            {
                directionData = dependencies.DirectionsRequired.GetNextInLine();
                DisplayNextSpin(directionData);
            }
        }

        private void CompleteEvent(bool success)
        {
            dependencies.CompleteEvent(new QuickTimeSpinWheelPayload(success));
            dependencies.SpinWheelController.ClearSubscriptions();
        }
    }
}