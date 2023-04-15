using UnityEngine;
using UnityEngine.Serialization;

namespace Core.MotorTest.Scripts
{
    public class MotorTestSceneInitializer : MonoBehaviour, IMillRotatorDependencies
    {
        [SerializeField] private MillRotator millRotator;
        [FormerlySerializedAs("tachoMotorController")] [SerializeField] private WaterMillMotorController waterMillMotorController;

        public void Awake()
        {
            waterMillMotorController.WaitForInitialize(MotorInitialized);
        }

        private void MotorInitialized()
        {
            millRotator.Initialize(this);
        }

        public IVirtualMill Mill => waterMillMotorController;
    }
}
