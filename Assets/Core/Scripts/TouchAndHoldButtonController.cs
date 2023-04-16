using System;
using Helpers;
using UnityEngine;
using LEGOWirelessSDK;

namespace Core.Scripts
{
    public class TouchAndHoldButtonController : MonoBehaviour, IButtonTapController
    {
        public bool IsInitialized { get; private set; } = false;
        private ForceSensor forceSensor;
        private Action sensorInitialized;
        private ITimer timer;

        public void WaitForInitialize(Action newSensorInitialized)
        {
            sensorInitialized = newSensorInitialized;
            actionToSubscribe = (t) => { };
            timer = gameObject.AddComponent<TimerFixedUpdateLoop>();
            Reset();
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
                timer.StartTimer(2f, CheckIfStillBeingHeld, HoldComplete);
            }
        }

        private void CheckIfStillBeingHeld(float t)
        {
            if (!timer.IsRunning) return;
            if (forceSensor.Force < 10)
            {
                actionToSubscribe?.Invoke(new QuickTimeTouchAndHoldButtonEventPayload(false));
                timer.StopTimer();
            }
        }

        private void HoldComplete()
        {
            actionToSubscribe?.Invoke(new QuickTimeTouchAndHoldButtonEventPayload(true));
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
