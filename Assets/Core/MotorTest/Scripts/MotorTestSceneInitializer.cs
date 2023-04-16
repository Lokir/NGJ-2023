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
        IQuickTimeTapButtonEventDependencies
    {
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
        [SerializeField] private QuickTimeTapButtonEvent quickTimeTapButtonEvent;
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
            TapController = gameObject.AddComponent<DummyButtonTapController>();
            CompleteEvent = QuickTimeEventCompleted;
            waterMillMotorController.WaitForInitialize(MotorInitialized);
            spinWheelEvent.Initialize(this);
            quickTimeTapButtonEvent.Initialize(this);
        }

        private void StartEventCycle()
        {
            quickTimeTapButtonEvent.PlayEvent();
        }
        
        private void MotorInitialized()
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
