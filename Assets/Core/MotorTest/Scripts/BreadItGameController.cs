using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts;
using FMODUnity;
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
            SpinDirections directionsRequired, QuicktimeShower shower, StudioEventEmitter lossSoundEmitter, StudioEventEmitter winSoundEmitter)
        {
            CompleteEvent = completeEvent;
            SpinWheelController = spinWheelController;
            DirectionsRequired = directionsRequired;
            Shower = shower;
            LossSoundEmitter = lossSoundEmitter;
            WinSoundEmitter = winSoundEmitter;
        }

        public QuicktimeShower Shower { get; }
        public StudioEventEmitter LossSoundEmitter { get; }
        public StudioEventEmitter WinSoundEmitter { get; }
        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public ISpinWheelInDirectionController SpinWheelController { get; }
        public SpinDirections DirectionsRequired { get; }
    }

    public class QuickTimeTapButtonDependencies : IQuickTimeTapButtonEventDependencies
    {
        public QuickTimeTapButtonDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            IButtonTapController tapController, QuicktimeShower shower, StudioEventEmitter lossSoundEmitter, StudioEventEmitter winSoundEmitter)
        {
            CompleteEvent = completeEvent;
            TapController = tapController;
            Shower = shower;
            LossSoundEmitter = lossSoundEmitter;
            WinSoundEmitter = winSoundEmitter;
        }

        public QuicktimeShower Shower { get; }
        public StudioEventEmitter LossSoundEmitter { get; }
        public StudioEventEmitter WinSoundEmitter { get; }
        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public IButtonTapController TapController { get; }
    }

    public class QuickTimeTouchAndHoldButtonEventDependencies : IQuickTimeTouchAndHoldButtonEventDependencies
    {
        public QuickTimeTouchAndHoldButtonEventDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            IButtonTapController touchAndHoldController, QuicktimeShower shower, StudioEventEmitter lossSoundEmitter, StudioEventEmitter winSoundEmitter)
        {
            CompleteEvent = completeEvent;
            TouchAndHoldController = touchAndHoldController;
            Shower = shower;
            LossSoundEmitter = lossSoundEmitter;
            WinSoundEmitter = winSoundEmitter;
        }

        public QuicktimeShower Shower { get; }
        public StudioEventEmitter LossSoundEmitter { get; }
        public StudioEventEmitter WinSoundEmitter { get; }
        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public IButtonTapController TouchAndHoldController { get; }
    }

    public class CrankQuickTimeEventDependencies : ICrankQuickTimeEventDependencies
    {
        public CrankQuickTimeEventDependencies(
            Action<IQuickTimeEventPayload> completeEvent, 
            int timeAllowed, 
            int requiredPosition, 
            ICrankController crankController, QuicktimeShower shower, StudioEventEmitter lossSoundEmitter, StudioEventEmitter winSoundEmitter)
        {
            CompleteEvent = completeEvent;
            TimeAllowed = timeAllowed;
            RequiredPosition = requiredPosition;
            CrankController = crankController;
            Shower = shower;
            LossSoundEmitter = lossSoundEmitter;
            WinSoundEmitter = winSoundEmitter;
        }

        public QuicktimeShower Shower { get; }
        public StudioEventEmitter LossSoundEmitter { get; }
        public StudioEventEmitter WinSoundEmitter { get; }
        public Action<IQuickTimeEventPayload> CompleteEvent { get; }
        public int TimeAllowed { get; }
        public int RequiredPosition { get; }
        public ICrankController CrankController { get; }
    }
    public class BreadItGameController : 
        MonoBehaviour, 
        IMillRotatorDependencies
    {

        [SerializeField] private EndScoreManager endScoreManager;
        
        [SerializeField] private QuicktimeShower shower;
        [SerializeField] private StudioEventEmitter lossSoundEmitter;
        [SerializeField] private StudioEventEmitter winSoundEmitter;
        
        [SerializeField] private CrankController crankController;
        [SerializeField] private TouchAndHoldButtonController touchAndHoldButtonController;
        [SerializeField] private TouchButtonController touchButtonController;
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
        [SerializeField] private QuickTimeTapButtonEvent quickTimeTapButtonEvent;
        [SerializeField] private QuickTimeTouchAndHoldButtonEvent touchAndHoldButtonEvent;
        [SerializeField] private CrankQuickTimeEvent crankQuickTimeEvent;

        public bool IsReady { get; private set; } = false;
        
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
                    touchAndHoldButtonEvent.Initialize(new QuickTimeTouchAndHoldButtonEventDependencies(QuickTimeEventCompleted, TouchAndHoldController, shower, lossSoundEmitter, winSoundEmitter));
                    break;
                case IQuickTimeTapButtonEvent:
                    quickTimeTapButtonEvent.Initialize(new QuickTimeTapButtonDependencies(QuickTimeEventCompleted, TapController, shower, lossSoundEmitter, winSoundEmitter));
                    break;
                case ICrankQuickTimeEvent:
                    crankQuickTimeEvent.Initialize(
                        new CrankQuickTimeEventDependencies(
                        QuickTimeEventCompleted, 
                        15, 
                        ProvideCrankRange(),
                        crankController, shower, lossSoundEmitter, winSoundEmitter));
                    break;
                case IQuickTimeSpinWheelEvent:
                    spinWheelEvent.Initialize(
                        new QuickTimeSpinWheelDependencies(
                            QuickTimeEventCompleted,
                            waterMillMotorController,
                            ProvideSpinDirections(), shower, lossSoundEmitter, winSoundEmitter));
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
                IsReady = true;
            }
        }

        public void StartGame()
        {
            if (!IsReady) return;
            idx = 0;
            eventsToGoThrough = eventsToGoThrough.OrderBy(a => Guid.NewGuid()).ToList();
            ProgressEventCycle();
        }

        public IVirtualMill Mill => waterMillMotorController;
        public Action<IQuickTimeEventPayload> CompleteEvent { get; private set; }

        private void QuickTimeEventCompleted(IQuickTimeEventPayload payload)
        {
            endScoreManager.UpdateScore((payload.Success ? 1 : 0));
            idx++;
            if(idx < eventsToGoThrough.Count)
                ProgressEventCycle();
            else 
                endScoreManager.GameOver();
        }
    }
}
