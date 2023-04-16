using System;

namespace Core.Scripts
{
    public interface IButtonTapController 
    {
        void SubscribeToSpinDirectionEvent(Action<IQuickTimeEventPayload> actionToSubscribe);
        void ClearSubscriptions();
        void Reset();
    }
}
