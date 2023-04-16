using System;
using UnityEngine;
using LEGOWirelessSDK;

namespace Core.Scripts
{
    public class TouchButtonController : MonoBehaviour, IButtonTapController
    {
        public bool IsInitialized { get; private set; } = false;
        private ForceSensor forceSensor;
        private Action sensorInitialized;

        public void WaitForInitialize(Action newSensorInitialized)
        {
            sensorInitialized = newSensorInitialized;
            actionToSubscribe = (t) => { };
        }

        public void OnSensorInitialize()
        {
            IsInitialized = true;
            forceSensor = GetComponent<ForceSensor>();
            sensorInitialized.Invoke();
        }

        private bool didPressThisCycle = false;
        public void SensorInputChanged()
        {
            if (didPressThisCycle) return;
            if (forceSensor.Force > 60)
            {
                didPressThisCycle = true;
                actionToSubscribe?.Invoke(new QuickTimeTapButtonEventPayload(true));
            }
        }
        
        private Action<IQuickTimeEventPayload> actionToSubscribe;
        public void SubscribeToSpinDirectionEvent(
            Action<IQuickTimeEventPayload> actionToSubscribe)
        {
            this.actionToSubscribe = actionToSubscribe;
        }

        public void ClearSubscriptions()
        {
            this.actionToSubscribe = null;
        }

        public void Reset()
        {
            didPressThisCycle = false;
        }
    }
}
