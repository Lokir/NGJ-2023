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

        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is ICrankQuickTimeEventDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.CrankController.SubscribeToSpinDirectionEvent(CrankCalled);
                crankTimer = gameObject.AddComponent<TimerFixedUpdateLoop>();
            }
            else throw new ArgumentException("Wrong dependency type given.");
        }

        public override void PlayEvent()
        {
            Debug.Log("Crank it!");
            cummulativeTargetPosition = 0;
            crankTimer.StartTimer(dependencies.TimeAllowed, null, EventComplete);
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private int cummulativeTargetPosition = 0;
        private void CrankCalled(ICrankDirectionPayload payload)
        {
            cummulativeTargetPosition = payload.Position;
        }
        
        private void EventComplete()
        {
            dependencies.CrankController.Reset();
            CompleteEvent(dependencies.RequiredPosition < cummulativeTargetPosition);
        }
        
        private void CompleteEvent(bool success)
        {
            dependencies.CompleteEvent(new QuickTimeTapButtonEventPayload(success));
            dependencies.CrankController.ClearSubscriptions();
        }
    }
}
