using System;
using UnityEngine;

namespace Core.MotorTest.Scripts
{
    public interface IMillRotatorDependencies
    {
        IVirtualMill Mill { get; }
    }
    public class MillRotator : MonoBehaviour
    {
        private Transform TargetRotation;
        private bool _isInitialized = false;
        private IMillRotatorDependencies dependencies;
        public void Initialize(IMillRotatorDependencies dependencies)
        {
            this.dependencies = dependencies;
            this.dependencies.Mill.SubscribeToOnWheelChanged(OnMillRotationChanged);
            TargetRotation = new GameObject("MillTargetRotation").transform;
            TargetRotation.rotation = this.transform.rotation;
            _isInitialized = true;
        }
        
        private void OnMillRotationChanged(IWheelChangeEventPayload payload)
        {
            if (payload.Stopped) return;
            TargetRotation.Rotate(Vector3.forward, payload.NewPosition);
        }

        private void Update()
        {
            if (!_isInitialized) return;
            transform.rotation = Quaternion.Slerp(TargetRotation.rotation, this.transform.rotation, 100*Time.deltaTime);
        }
    }
}