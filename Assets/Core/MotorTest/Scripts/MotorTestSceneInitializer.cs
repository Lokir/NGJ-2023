using System;
using System.Collections.Generic;
using Core.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.MotorTest.Scripts
{
    public class MotorTestSceneInitializer : MonoBehaviour, IMillRotatorDependencies, IQuickTimeSpinWheelDependencies
    {
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;
        [SerializeField] private QuickTimeSpinWheelEvent spinWheelEvent;
        public void Awake()
        {
            CompleteEvent = QuickTimeEventCompleted;
            waterMillMotorController.WaitForInitialize(MotorInitialized);
            spinWheelEvent.Initialize(this);
        }

        private void MotorInitialized()
        {
            spinWheelEvent.PlayEvent();
        }

        public IVirtualMill Mill => waterMillMotorController;
        public Action<IQuickTimeEventPayload> CompleteEvent { get; private set; }

        private void QuickTimeEventCompleted(IQuickTimeEventPayload payload)
        {
            Debug.Log("Completed Event");
        }

        public ISpinWheelInDirectionController SpinWheelController => waterMillMotorController;

        public SpinDirections DirectionsRequired => new (
            new List<SpinDirectionData>()
            {
                new SpinDirectionData(SpinDirection.Backward, 3), 
                new SpinDirectionData(SpinDirection.Forward, 3) , 
                new SpinDirectionData(SpinDirection.Either, 3),
            });
    }
}
