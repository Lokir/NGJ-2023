namespace LEGODeviceUnitySDK
{
    public interface ILEGOChargingLightDelegate : ILEGOServiceDelegate
    {
        void DidUpdateChargingLightColor(LEGOChargingLight chargingLight, LEGOValue oldValue, LEGOValue newValue);
    }
}