using System;
using System.Collections.Generic;
using Core.Scripts;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.MotorTest.Scripts
{
    public class MotorTestSceneInitializer : MonoBehaviour, IMillRotatorDependencies, IQuickTimeSpinWheelDependencies
    {
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
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
            CompleteEvent = QuickTimeEventCompleted;
            waterMillMotorController.WaitForInitialize(MotorInitialized);
            spinWheelEvent.Initialize(this);
        }

        private void StartEventCycle()
        {
            spinWheelEvent.PlayEvent();
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
