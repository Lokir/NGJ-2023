using System;
using UnityEngine;

namespace Core.Scripts
{
    public class DummyButtonTapController : MonoBehaviour, IButtonTapController
    {
        private Action<IQuickTimeTapButtonEventPayload> actionToSubscribe;
        public void SubscribeToSpinDirectionEvent(
            Action<IQuickTimeTapButtonEventPayload> actionToSubscribe)
        {
            this.actionToSubscribe = actionToSubscribe;
        }

        public void ClearSubscriptions()
        {
            this.actionToSubscribe = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                actionToSubscribe?.Invoke(new QuickTimeTapButtonEventPayload(true));
            }
        }
    }
}
