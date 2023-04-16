using System;
using UnityEngine;

namespace Core.Scripts
{
    public class DummyButtonTapController : MonoBehaviour, IButtonTapController
    {
        public void SubscribeToSpinDirectionEvent(Action<IQuickTimeTapButtonEventPayload> actionToSubscribe)
        {
            throw new NotImplementedException();
        }
    }
}
