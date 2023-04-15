namespace LEGODeviceUnitySDK
{
    public interface ILEGOTechnic3AxisGestureSensorDelegate : ILEGOServiceDelegate
    {
        void DidUpdateGesture(LEGOTechnic3AxisGestureSensor sensor, LEGOValue oldValue, LEGOValue newValue);
    }
}