using System;
using Helpers;
using LEGOWirelessSDK;
using UnityEngine;

namespace Core.Scripts
{
    public interface ICrankController
    {
        int MotorPosition { get; }
        void SubscribeToSpinDirectionEvent(Action<ICrankDirectionPayload> actionToSubscribe);
        void ClearSubscriptions();
        void Reset();
    }
    
    public class CrankController : MonoBehaviour, ICrankController
    {
        public bool IsInitialized { get; private set; } = false;
        private Action sensorInitialized;
        private Action<ICrankDirectionPayload> actionToSubscribe;
        private TachoMotor motor;

        public void WaitForInitialize(Action newSensorInitialized)
        {
            sensorInitialized = newSensorInitialized;
            actionToSubscribe = (t) => { };
        }

        public void OnSensorInitialize()
        {
            IsInitialized = true;
            motor = GetComponent<TachoMotor>();
            sensorInitialized.Invoke();
        }

        public void OnPositionChanged()
        {
            actionToSubscribe.Invoke(new CrankDirectionPayload(motor.Position));
        }
        
        private SpinDirection GetCurrentDirectionBasedOnMovement()
        {
            if (motor.Speed > 0) return SpinDirection.Forward;
            if (motor.Speed < 0) return SpinDirection.Backward;
            return SpinDirection.None;
        }

        public int MotorPosition => motor.Position;

        public void SubscribeToSpinDirectionEvent(Action<ICrankDirectionPayload> actionToSubscribe)
        {
            this.actionToSubscribe = actionToSubscribe;
        }

        public void ClearSubscriptions()
        {
            this.actionToSubscribe = null;
        }

        public void Reset()
        {
            motor.ResetPosition();
        }
    }
}
