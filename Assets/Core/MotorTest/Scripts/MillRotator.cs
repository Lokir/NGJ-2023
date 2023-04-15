using UnityEngine;

namespace Core.MotorTest.Scripts
{
    public interface IMillRotatorDependencies
    {
        IVirtualMill Mill { get; }
    }
    public class MillRotator : MonoBehaviour
    {
        private bool _isInitialized = false;
        private IMillRotatorDependencies dependencies;
        public void Initialize(IMillRotatorDependencies dependencies)
        {
            this.dependencies = dependencies;
            this.dependencies.Mill.SubscribeToOnWheelChanged(OnMillRotationChanged);
            _isInitialized = true;
        }

        private void OnMillRotationChanged(IWheelChangeEventPayload payload)
        {
            if (payload.Stopped) return;
            transform.Rotate(Vector3.forward, payload.NewPosition);
        }
    }
}