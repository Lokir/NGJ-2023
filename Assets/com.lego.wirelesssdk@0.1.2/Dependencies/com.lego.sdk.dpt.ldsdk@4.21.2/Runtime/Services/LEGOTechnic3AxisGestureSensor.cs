namespace LEGODeviceUnitySDK
{
    public class LEGOTechnic3AxisGestureSensor : LEGOService, ILEGOTechnic3AxisGestureSensor
    {
        public LEGOTechnic3AxisGestureSensor(Builder builder) : base(builder)
        {
            
        }

        public override string ServiceName
        {
            get
            {
                return "Technic Three Axis Gesture Sensor";
            }
        }

        protected override int DefaultModeNumber
        {
            get
            {
                return (int) Technic3AxisGestureSensorMode.Gesture;
            }
        }

        public Technic3AxisGestureSensorMode Mode
        {
            get
            {
                return InputFormat == null
                    ? Technic3AxisGestureSensorMode.Unknown
                    : (Technic3AxisGestureSensorMode) InputFormat.Mode;
            }
        }

        public enum Technic3AxisGestureSensorMode
        {
            Gesture = 0,
            Unknown = -1,
        }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((Technic3AxisGestureSensorMode) newValue.Mode)
            {
                case Technic3AxisGestureSensorMode.Gesture:
                    _delegates.OfType<ILEGOTechnic3AxisGestureSensorDelegate>().ForEach(serviceDelegate =>
                        serviceDelegate.DidUpdateGesture(this, oldValue, newValue));
                    break;
                case Technic3AxisGestureSensorMode.Unknown:
                default:
                    break;
            }
        }
    }
}