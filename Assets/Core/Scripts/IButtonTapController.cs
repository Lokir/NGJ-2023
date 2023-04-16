using System;

namespace Core.Scripts
{
    public interface IButtonTapController 
    {
        void SubscribeToSpinDirectionEvent(Action<IQuickTimeTapButtonEventPayload> actionToSubscribe);
        void ClearSubscriptions();
    }
}
