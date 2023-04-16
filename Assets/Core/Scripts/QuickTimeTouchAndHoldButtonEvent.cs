using System;
using UnityEngine;

namespace Core.Scripts
{
    public interface IQuickTimeTouchAndHoldButtonEvent : IQuickTimeEvent
    {
    
    }

    public interface IQuickTimeTouchAndHoldButtonEventDependencies : IQuickTimeEventDependencies
    {
        IButtonTapController TouchAndHoldController { get; }
    }

    public interface IQuickTimeTouchAndHoldButtonEventPayload : IQuickTimeEventPayload
    {
        
    }

    public class QuickTimeTouchAndHoldButtonEventPayload : IQuickTimeTouchAndHoldButtonEventPayload
    {
        public QuickTimeTouchAndHoldButtonEventPayload(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
    public class QuickTimeTouchAndHoldButtonEvent : QuickTimeEvent, IQuickTimeTouchAndHoldButtonEvent
    {
        private IQuickTimeTouchAndHoldButtonEventDependencies dependencies;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is IQuickTimeTouchAndHoldButtonEventDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.TouchAndHoldController.SubscribeToSpinDirectionEvent(ButtonTapped);

            }
            else throw new ArgumentException("Wrong dependency type given.");
        }


        public override void PlayEvent()
        {
            Debug.Log("Hold the button!");
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private void ButtonTapped(IQuickTimeEventPayload payload)
        {
            CompleteEvent(payload.Success);
        }
        
        private void CompleteEvent(bool success)
        {
            dependencies.CompleteEvent(new QuickTimeTapButtonEventPayload(success));
            dependencies.TouchAndHoldController.ClearSubscriptions();
        }
    }
}