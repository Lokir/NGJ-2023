using System;
using Helpers;
using UnityEngine;

namespace Core.Scripts
{
    public interface ICrankQuickTimeEvent : IQuickTimeEvent
    {
        
    }

    public interface ICrankDirectionPayload
    {
        int Position { get; }
    }

    public class CrankDirectionPayload : ICrankDirectionPayload
    {
        public CrankDirectionPayload(int position)
        {
            Position = position;
        }

        public int Position { get; }
        public SpinDirection Direction { get; }
    }
    public interface ICrankQuickTimeEventDependencies : IQuickTimeEventDependencies
    {
        int TimeAllowed { get; }
        int RequiredPosition { get; }
        ICrankController CrankController { get; }
    }
    public class CrankQuickTimeEvent : QuickTimeEvent, ICrankQuickTimeEvent
    {
        private ITimer crankTimer;
        private ICrankQuickTimeEventDependencies dependencies;
        private int motorStartPosition;
        private int motorRequiredPosition;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is ICrankQuickTimeEventDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.CrankController.SubscribeToSpinDirectionEvent(CrankCalled);
                crankTimer = gameObject.AddComponent<TimerFixedUpdateLoop>();
                cummulativeTargetPosition = 0;
            }
            else throw new ArgumentException("Wrong dependency type given.");
        }

        public override void PlayEvent()
        {
            motorStartPosition = dependencies.CrankController.MotorPosition;
            motorRequiredPosition = motorStartPosition + dependencies.RequiredPosition;
            Debug.Log($"Crank it! {dependencies.RequiredPosition}");
            cummulativeTargetPosition = 0;
            crankTimer.StartTimer(dependencies.TimeAllowed, CrankTick, EventComplete);
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private int cummulativeTargetPosition = 0;
        private void CrankCalled(ICrankDirectionPayload payload)
        {
            cummulativeTargetPosition = payload.Position;
            Debug.Log(cummulativeTargetPosition);
        }

        private void CrankTick(float t)
        {
            if (cummulativeTargetPosition > motorRequiredPosition)
            {
                crankTimer.StopTimer();
                EventComplete();
            }
        }
        
        private void EventComplete()
        {
            CompleteEvent(motorRequiredPosition < cummulativeTargetPosition);
        }
        
        private void CompleteEvent(bool success)
        {
            dependencies.CompleteEvent(new QuickTimeTapButtonEventPayload(success));
            dependencies.CrankController.ClearSubscriptions();
        }
    }
}
