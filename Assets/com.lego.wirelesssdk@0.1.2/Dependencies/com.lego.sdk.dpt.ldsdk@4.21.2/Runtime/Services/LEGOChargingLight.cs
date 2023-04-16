namespace LEGODeviceUnitySDK
{
    public class LEGOChargingLight : LEGOService, ILEGORGBLight
    {
        public LEGOChargingLight(Builder builder) : base(builder)
        {
        }

        public override string ServiceName
        {
            get
            {
                return "Charging Light";
            }
        }

        protected override int DefaultModeNumber
        {
            get
            {
                return (int) ChargingLightMode.Color;
            }
        }

        public ChargingLightMode Mode
        {
            get
            {
                return InputFormat == null ? ChargingLightMode.Unknown : (ChargingLightMode) InputFormat.Mode;
            }
        }

        public enum ChargingLightMode
        {
            Color   =  0,
            Unknown = -1,
        }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((ChargingLightMode) newValue.Mode)
            {
                case ChargingLightMode.Color:
                    _delegates.OfType<ILEGOChargingLightDelegate>().ForEach(serviceDelegate =>
                        serviceDelegate.DidUpdateChargingLightColor(this, oldValue, newValue));
                    break;
                case ChargingLightMode.Unknown:
                default:
                    break;
            }
        }
    }
}
