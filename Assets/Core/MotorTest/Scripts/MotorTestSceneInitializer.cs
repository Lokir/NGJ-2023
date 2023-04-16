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
        IQuickTimeTouchAndHoldButtonEventDependencies,
        ICrankQuickTimeEventDependencies
    {
        [SerializeField] private CrankController crankController;
        [SerializeField] private TouchAndHoldButtonController touchAndHoldButtonController;
        [SerializeField] private TouchButtonController touchButtonController;
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
        [SerializeField] private QuickTimeTapButtonEvent quickTimeTapButtonEvent;
        [SerializeField] private QuickTimeTouchAndHoldButtonEvent touchAndHoldButtonEvent;
        [SerializeField] private CrankQuickTimeEvent crankQuickTimeEvent;
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
            crankController.WaitForInitialize(CrankInitialize);
            // spinWheelEvent.Initialize(this);
            // quickTimeTapButtonEvent.Initialize(this);
            //touchAndHoldButtonEvent.Initialize(this);
            crankQuickTimeEvent.Initialize(this);
        }

        private void StartEventCycle()
        {
            crankQuickTimeEvent.PlayEvent();
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

        private void CrankInitialize()
        {
            WaitForAllSensorsInitialized("crank");
        }

        private bool buttonInitialized = false;
        private bool wheelMotorInitialized = false;
        private bool touchAndHoldInitialized = false;
        private bool crankInitialized  = false;
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
                case "crank":
                    crankInitialized = true;
                    break;
                default: throw new ArgumentException("unkown sensor");
            }

            if (buttonInitialized && wheelMotorInitialized && touchAndHoldInitialized && crankInitialized)
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
            Debug.Log($"Completed Event: {payload.Success}");
        }
        public ISpinWheelInDirectionController SpinWheelController => waterMillMotorController;
        public SpinDirections DirectionsRequired { get; private set; }
        public int TimeAllowed => 10;
        public int RequiredPosition => 720;
        public ICrankController CrankController => crankController;
    }
}
