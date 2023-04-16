namespace LEGODeviceUnitySDK
{
    public interface ILEGODeviceConfig
    {
        bool IgnoreFailedInterrogationEvents { get; }
    }

    public class LEGODeviceConfig : ILEGODeviceConfig
    {
        public bool IgnoreFailedInterrogationEvents { get; private set; } = false; //Explicit false.

        public LEGODeviceConfig() { }

        public LEGODeviceConfig(bool ignoreFailedInterrogationEvents)
        {
            IgnoreFailedInterrogationEvents = ignoreFailedInterrogationEvents;
        }
    }
}