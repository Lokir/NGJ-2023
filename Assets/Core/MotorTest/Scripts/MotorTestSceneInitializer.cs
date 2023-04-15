using UnityEngine;

namespace Core.MotorTest.Scripts
{
    public class MotorTestSceneInitializer : MonoBehaviour, IMillRotatorDependencies
    {
        [SerializeField] private MillRotator millRotator;
        [SerializeField] private TachoMotorController tachoMotorController;

        public void Awake()
        {
            tachoMotorController.WaitForInitialize(MotorInitialized);
        }

        private void MotorInitialized()
        {
            millRotator.Initialize(this);
        }

        public IVirtualMill Mill => tachoMotorController;
    }
}
