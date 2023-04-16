using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Core.MotorTest.Scripts
{
    public class QuickTimeSpinWheelDependencies : IQuickTimeSpinWheelDependencies
    {
        public QuickTimeSpinWheelDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            ISpinWheelInDirectionController spinWheelController, 
            SpinDirections directionsRequired)
        {
            CompleteEvent = completeEvent;
            SpinWheelController = spinWheelController;
            DirectionsRequired = directionsRequired;
        }

        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public ISpinWheelInDirectionController SpinWheelController { get; }
        public SpinDirections DirectionsRequired { get; }
    }

    public class QuickTimeTapButtonDependencies : IQuickTimeTapButtonEventDependencies
    {
        public QuickTimeTapButtonDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            IButtonTapController tapController)
        {
            CompleteEvent = completeEvent;
            TapController = tapController;
        }

        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public IButtonTapController TapController { get; }
    }

    public class QuickTimeTouchAndHoldButtonEventDependencies : IQuickTimeTouchAndHoldButtonEventDependencies
    {
        public QuickTimeTouchAndHoldButtonEventDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            IButtonTapController touchAndHoldController)
        {
            CompleteEvent = completeEvent;
            TouchAndHoldController = touchAndHoldController;
        }

        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public IButtonTapController TouchAndHoldController { get; }
    }

    public class CrankQuickTimeEventDependencies : ICrankQuickTimeEventDependencies
    {
        public CrankQuickTimeEventDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            int timeAllowed, 
            int requiredPosition, 
            ICrankController crankController)
        {
            CompleteEvent = completeEvent;
            TimeAllowed = timeAllowed;
            RequiredPosition = requiredPosition;
            CrankController = crankController;
        }

        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public int TimeAllowed { get; }
        public int RequiredPosition { get; }
        public ICrankController CrankController { get; }
    }
    public class MotorTestSceneInitializer : 
        MonoBehaviour, 
        IMillRotatorDependencies
    {
        [SerializeField] private CrankController crankController;
        [SerializeField] private TouchAndHoldButtonController touchAndHoldButtonController;
        [SerializeField] private TouchButtonController touchButtonController;
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
        [SerializeField] private QuickTimeTapButtonEvent quickTimeTapButtonEvent;
        [SerializeField] private QuickTimeTouchAndHoldButtonEvent touchAndHoldButtonEvent;
        [SerializeField] private CrankQuickTimeEvent crankQuickTimeEvent;

        private int idx = 0;
        [SerializeField] private List<QuickTimeEvent> eventsToGoThrough;
        public IButtonTapController TouchAndHoldController { get; private set; }
        public IButtonTapController TapController { get; private set; }
        private ITimer timer;
        public void Awake()
        {
            timer = gameObject.AddComponent<TimerFixedUpdateLoop>();
            TapController = touchButtonController;
            TouchAndHoldController = touchAndHoldButtonController;
            CompleteEvent = QuickTimeEventCompleted;
            waterMillMotorController.WaitForInitialize(MotorInitialized);
            touchButtonController.WaitForInitialize(TouchSensorInitialized);
            touchAndHoldButtonController.WaitForInitialize(TouchAndHoldSensorInitialized);
            crankController.WaitForInitialize(CrankInitialize);
        }

        private void InitializeEvent(QuickTimeEvent quickTimeEvent)
        {
            switch (quickTimeEvent)
            {
                case IQuickTimeTouchAndHoldButtonEvent:
                    touchAndHoldButtonEvent.Initialize(new QuickTimeTouchAndHoldButtonEventDependencies(QuickTimeEventCompleted, TouchAndHoldController));
                    break;
                case IQuickTimeTapButtonEvent:
                    quickTimeTapButtonEvent.Initialize(new QuickTimeTapButtonDependencies(QuickTimeEventCompleted, TapController));
                    break;
                case ICrankQuickTimeEvent:
                    crankQuickTimeEvent.Initialize(
                        new CrankQuickTimeEventDependencies(
                        QuickTimeEventCompleted, 
                        15, 
                        ProvideCrankRange(),
                        crankController));
                    break;
                case IQuickTimeSpinWheelEvent:
                    spinWheelEvent.Initialize(
                        new QuickTimeSpinWheelDependencies(
                            QuickTimeEventCompleted,
                            waterMillMotorController,
                            ProvideSpinDirections()));
                    break;
            }
        }

        private int ProvideCrankRange()
        {
            var rand = Random.Range(720, 2000);
            if (rand % 2 == 0) rand *= -1;
            return rand;
        }

        private SpinDirections ProvideSpinDirections()
        {
            var spinDirectionData = new List<SpinDirectionData>();
            var random = Random.Range(0, 100);
                var direction = random > 50 ? SpinDirection.Backward : SpinDirection.Forward;
                spinDirectionData.Add( new SpinDirectionData(direction, 3));
            return new SpinDirections(
            spinDirectionData);
        }
        
        private void ProgressEventCycle()
        {
            var quickTimeEvent = eventsToGoThrough[idx];
            InitializeEvent(quickTimeEvent);
            quickTimeEvent.PlayEvent();
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
            idx = 0;
            eventsToGoThrough = eventsToGoThrough.OrderBy(a => Guid.NewGuid()).ToList();
            timer.StartTimer(0.2f, null, ProgressEventCycle);
        }

        public IVirtualMill Mill => waterMillMotorController;
        public Action<IQuickTimeEventPayload> CompleteEvent { get; private set; }

        private void QuickTimeEventCompleted(IQuickTimeEventPayload payload)
        {
            Debug.Log($"Completed Event: {payload.Success}");
            Debug.Log($"Added point value {(payload.Success ? 1: 0)}");
            idx++;
            if(idx < eventsToGoThrough.Count)
                ProgressEventCycle();
            else 
                Debug.Log("Game completed");
        }

        public void ResetEventList()
        {
            eventsToGoThrough = eventsToGoThrough.OrderBy(a => Guid.NewGuid()).ToList();
            idx = 0;
        }
    }
}
