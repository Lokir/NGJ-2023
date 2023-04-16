using System;
using UnityEngine;

namespace Core.Scripts
{
    public interface IQuickTimeTapButtonEvent : IQuickTimeEvent
    {
    
    }

    public interface IQuickTimeTapButtonEventDependencies : IQuickTimeEventDependencies
    {
        IButtonTapController TapController { get; }
    }

    public interface IQuickTimeTapButtonEventPayload : IQuickTimeEventPayload
    {
        
    }

    public class QuickTimeTapButtonEventPayload : IQuickTimeTapButtonEventPayload
    {
        public QuickTimeTapButtonEventPayload(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
    public class QuickTimeTapButtonEvent : QuickTimeEvent, IQuickTimeTapButtonEvent
    {
        private IQuickTimeTapButtonEventDependencies dependencies;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is IQuickTimeTapButtonEventDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.TapController.SubscribeToSpinDirectionEvent(ButtonTapped);

            }
            else throw new ArgumentException("Wrong dependency type given.");
        }


        public override void PlayEvent()
        {
            Debug.Log("Push the button!");
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private void ButtonTapped(IQuickTimeTapButtonEventPayload payload)
        {
            CompleteEvent(payload.Success);
        }
        
        private void CompleteEvent(bool success)
        {
            dependencies.CompleteEvent(new QuickTimeTapButtonEventPayload(success));
            dependencies.TapController.ClearSubscriptions();
        }
    }
}
