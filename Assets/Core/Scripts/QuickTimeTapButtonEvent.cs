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
        [SerializeField] private Sprite tapImage;

        private IQuickTimeTapButtonEventDependencies dependencies;
        public override void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            if (quicktimeDependencies is IQuickTimeTapButtonEventDependencies newDependencies)
            {
                dependencies = newDependencies;
                dependencies.TapController.SubscribeToSpinDirectionEvent(ButtonTapped);
                dependencies.TapController.Reset();

            }
            else throw new ArgumentException("Wrong dependency type given.");
        }


        public override void PlayEvent()
        {
            dependencies.Shower.Show(tapImage);
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
            dependencies.TapController.ClearSubscriptions();
            Invoke("DelayedCompleteEvent", 2f);
        }
        
        private void DelayedCompleteEvent()
        {
            dependencies.CompleteEvent(new QuickTimeSpinWheelPayload(_success));
        }
    }
}
