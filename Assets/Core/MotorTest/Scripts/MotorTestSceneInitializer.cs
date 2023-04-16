using System;
using System.Collections.Generic;
using Core.Scripts;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.MotorTest.Scripts
{
    public class MotorTestSceneInitializer : 
        MonoBehaviour, 
        IMillRotatorDependencies, 
        IQuickTimeSpinWheelDependencies,
        IQuickTimeTapButtonEventDependencies,
        IQuickTimeTouchAndHoldButtonEventDependencies
    {
        [SerializeField] private TouchAndHoldButtonController touchAndHoldButtonController;
        [SerializeField] private TouchButtonController touchButtonController;
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
        [SerializeField] private QuickTimeTapButtonEvent quickTimeTapButtonEvent;
        [SerializeField] private QuickTimeTouchAndHoldButtonEvent touchAndHoldButtonEvent;
        public IButtonTapController TouchAndHoldController { get; private set; }
        public IButtonTapController TapController { get; private set; }
        private ITimer timer;
        public void Awake()
        {
            DirectionsRequired = new SpinDirections(
                new List<SpinDirectionData>()
                {
                    new SpinDirectionData(SpinDirection.Backward, 3), 
                    new SpinDirectionData(SpinDirection.Forward, 3), 
                });
            timer = gameObject.AddComponent<TimerFixedUpdateLoop>();
            TapController = touchButtonController;
            TouchAndHoldController = touchAndHoldButtonController;
            CompleteEvent = QuickTimeEventCompleted;
            waterMillMotorController.WaitForInitialize(MotorInitialized);
            touchButtonController.WaitForInitialize(TouchSensorInitialized);
            touchAndHoldButtonController.WaitForInitialize(TouchAndHoldSensorInitialized);
            spinWheelEvent.Initialize(this);
            quickTimeTapButtonEvent.Initialize(this);
            touchAndHoldButtonEvent.Initialize(this);
        }

        private void StartEventCycle()
        {
            touchAndHoldButtonEvent.PlayEvent();
        }
        
        private void MotorInitialized()
        {
            WaitForAllSensorsInitialized("wheelMotor");
        }

        private void TouchSensorInitialized()
        {
            WaitForAllSensorsInitialized("button");
        }

        private void TouchAndHoldSensorInitialized()
        {
            WaitForAllSensorsInitialized("touchAndHoldButton");
        }

        private bool buttonInitialized;
        private bool wheelMotorInitialized;
        private bool touchAndHoldInitialized;
        private void WaitForAllSensorsInitialized(string sensor)
        {
            switch (sensor)
            {
                case "button":
                    buttonInitialized = true;
                    break;
                case "wheelMotor":
                    wheelMotorInitialized = true;
                    break;
                case "touchAndHoldButton":
                    touchAndHoldInitialized = true;
                    break;
                default: throw new ArgumentException("unkown sensor");
            }

            if (buttonInitialized && wheelMotorInitialized && touchAndHoldInitialized)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            timer.StartTimer(0.2f, null, StartEventCycle);
        }

        public IVirtualMill Mill => waterMillMotorController;
        public Action<IQuickTimeEventPayload> CompleteEvent { get; private set; }

        private void QuickTimeEventCompleted(IQuickTimeEventPayload payload)
        {
            Debug.Log("Completed Event");
        }

        public ISpinWheelInDirectionController SpinWheelController => waterMillMotorController;

        public SpinDirections DirectionsRequired { get; private set; }
    }
}
