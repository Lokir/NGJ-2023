using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public class LEGOLEDMatrix : LEGOService, ILEGOLEDMatrix
    {
        public override string ServiceName { get { return "LEDMatrix"; } }
        protected override int DefaultModeNumber { get { return (int)Mode.Level; } }

        internal LEGOLEDMatrix(LEGOService.Builder builder) : base(builder) { }
        
        public Mode MatrixMode { get { return InputFormat == null ? Mode.Unknown : (Mode)InputFormat.Mode; } }
        
        public class PixelConfiguration
        {
            public PixelConfiguration(int color, int intensity)
            {
                Color = color;
                Intensity = intensity;
            }
            public int Color;
            public int Intensity;
        }
        
        public enum Mode
        {
            Level = 0,
            Color = 1,
            Pixel = 2,
            Transition = 3,
            Unknown = -1,
        }
        
        public abstract class LEDMatrixCommand : LEGOServiceCommand
        {
            protected byte MODE_DIRECT_LEVEL      = 0x00;
            protected byte MODE_DIRECT_COLOR      = 0x01;
            protected byte MODE_DIRECT_PIXEL      = 0x02;
            protected byte MODE_DIRECT_TRANSITION = 0x03;
            

            public override ICollection<IOType> SupportedIOTypes
            {
                get { return IOTypes.LEDMatrices; }
            }
        }
        
        public class SetLevelCommand : LEDMatrixCommand
        {
            public int Level;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[]
                    {
                        MODE_DIRECT_LEVEL, 
                        (byte)EncodeLedMatrixLevel(Level)
                    });
            }
        }
        
        public class SetColorCommand : LEDMatrixCommand
        {
            public int Color;
            public int Transition;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[]
                    {
                        MODE_DIRECT_COLOR, 
                        (byte)EncodeLedMatrixColor(Color), 
                        (byte)EncodeLedMatrixColor(Color), 
                        (byte)EncodeLedMatrixTransitionColor(Transition)
                    });
            }
        }
        
        public class SetPixelCommand : LEDMatrixCommand
        {
            public PixelConfiguration[] intensityAndColorMatrix = new PixelConfiguration[9];

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[]
                    {
                        MODE_DIRECT_PIXEL,
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[0]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[1]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[2]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[3]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[4]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[5]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[6]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[7]),
                        (byte)EncodeLedMatrixColorAndIntensity(intensityAndColorMatrix[8]),
                        (byte)0x00,
                        (byte)0x00,
                    });
            }
        }
        
        public class SetTransitionCommand : LEDMatrixCommand
        {
            public int Transition;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[]
                    {
                        MODE_DIRECT_TRANSITION,
                        (byte)EncodeLedMatrixTransitionType(Transition),
                    });
            }
        }
    }
    



}
