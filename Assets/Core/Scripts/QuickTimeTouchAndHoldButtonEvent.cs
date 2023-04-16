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
        [SerializeField] private Sprite tapAndHoldImage;

        private IQuickTimeTouchAndHoldButtonEventDependencies dependencies;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is IQuickTimeTouchAndHoldButtonEventDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.TouchAndHoldController.SubscribeToSpinDirectionEvent(ButtonTapped);
                dependencies.TouchAndHoldController.Reset();

            }
            else throw new ArgumentException("Wrong dependency type given.");
        }


        public override void PlayEvent()
        {
            dependencies.Shower.Show(tapAndHoldImage);
        }

        public override void StopEvent()
        {
            CompleteEvent(false);
        }

        private void ButtonTapped(IQuickTimeEventPayload payload)
        {
            CompleteEvent(payload.Success);
        }
        private bool _success = false;

        private void CompleteEvent(bool success)
        {
            _success = success;
            if(success)
                dependencies.WinSoundEmitter.Play();
            else 
                dependencies.LossSoundEmitter.Play();
            dependencies.TouchAndHoldController.ClearSubscriptions();
            dependencies.TouchAndHoldController.Reset();
            Invoke("DelayedCompleteEvent", 2f);

        }
        private void DelayedCompleteEvent()
        {
            dependencies.CompleteEvent(new QuickTimeTapButtonEventPayload(_success));

        }
    }
}
