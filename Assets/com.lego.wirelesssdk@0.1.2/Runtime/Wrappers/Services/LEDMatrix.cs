// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LEGOWirelessSDK
{
    public class LEDMatrix : ServiceBase
    {
        // Row 0 is top row. Column 0 is left column.
        public void SetPixel(int row, int column, LEDMatrixColor color, int intensity) // 0 to 10.
        {
            if (matrix == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            intensityAndColorMatrix[3 * row + column].Color = (int)color;
            intensityAndColorMatrix[3 * row + column].Intensity = intensity;
            if (mode != LEGOLEDMatrix.Mode.Pixel)
            {
                matrix.UpdateInputFormat(new LEGOInputFormat(matrix.ConnectInfo.PortID, matrix.ioType, (int)LEGOLEDMatrix.Mode.Pixel, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
                mode = LEGOLEDMatrix.Mode.Pixel;
            }
            matrix.SendCommand(new LEGOLEDMatrix.SetPixelCommand() { intensityAndColorMatrix = intensityAndColorMatrix });
        }

        // Arrays are row major: First three values are top row, next three values are middle row, final three values are bottom row.
        public void SetPixels(LEDMatrixColor[] colors, int[] intensities) // 0 to 10.
        {
            if (matrix == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            for (var i = 0; i < 9; ++i)
            {
                intensityAndColorMatrix[i].Color = (int)colors[i];
                intensityAndColorMatrix[i].Intensity = intensities[i];
            }
            if (mode != LEGOLEDMatrix.Mode.Pixel)
            {
                matrix.UpdateInputFormat(new LEGOInputFormat(matrix.ConnectInfo.PortID, matrix.ioType, (int)LEGOLEDMatrix.Mode.Pixel, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
                mode = LEGOLEDMatrix.Mode.Pixel;
            }
            matrix.SendCommand(new LEGOLEDMatrix.SetPixelCommand() { intensityAndColorMatrix = intensityAndColorMatrix });
        }

        public void SetAllPixels(LEDMatrixColor color, int intensity) // 0 to 10.
        {
            if (matrix == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            for (var i = 0; i < 9; ++i)
            {
                intensityAndColorMatrix[i].Color = (int)color;
                intensityAndColorMatrix[i].Intensity = intensity;
            }
            if (mode != LEGOLEDMatrix.Mode.Pixel)
            {
                matrix.UpdateInputFormat(new LEGOInputFormat(matrix.ConnectInfo.PortID, matrix.ioType, (int)LEGOLEDMatrix.Mode.Pixel, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
                mode = LEGOLEDMatrix.Mode.Pixel;
            }
            matrix.SendCommand(new LEGOLEDMatrix.SetPixelCommand() { intensityAndColorMatrix = intensityAndColorMatrix });
        }

        public void SetPixelTransition(LEDMatrixTransition transition)
        {
            if (matrix == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            matrix.SendCommand(new LEGOLEDMatrix.SetTransitionCommand() { Transition = (int)transition });
        }

        public void SetLevel(int level) // -9 to 9.
        {
            if (matrix == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            if (mode != LEGOLEDMatrix.Mode.Level)
            {
                matrix.UpdateInputFormat(new LEGOInputFormat(matrix.ConnectInfo.PortID, matrix.ioType, (int)LEGOLEDMatrix.Mode.Level, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
                mode = LEGOLEDMatrix.Mode.Level;
            }
            matrix.SendCommand(new LEGOLEDMatrix.SetLevelCommand() { Level = level });

        }

        #region internals
        public enum LEDMatrixColor
        {
            Off = 0,
            Maroon = 1,
            Purple = 2,
            Blue = 3,
            Cyan = 4,
            SpringGreen = 5,
            Green = 6,
            Yellow = 7,
            Orange = 8,
            Red = 9,
            White = 10
        }

        public enum LEDMatrixTransition
        {
            None = 0,
            Scroll = 1,
            Fade = 2
        }

        private LEGOLEDMatrix matrix;

        private LEGOLEDMatrix.PixelConfiguration[] intensityAndColorMatrix = new LEGOLEDMatrix.PixelConfiguration[] {
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0),
        new LEGOLEDMatrix.PixelConfiguration((int)LEDMatrixColor.Off, 0)
    };

        private LEGOLEDMatrix.Mode mode = LEGOLEDMatrix.Mode.Pixel;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            matrix = services.FirstOrDefault(s => s.ioType == IOType.LEIOTypeGeckoLEDMatrix) as LEGOLEDMatrix;
            if (matrix == null)
            {
                Debug.LogWarning(name + " service not found");
                return false;
            }
            services.Remove(matrix);
            matrix.UpdateInputFormat(new LEGOInputFormat(matrix.ConnectInfo.PortID, matrix.ioType, (int)LEGOLEDMatrix.Mode.Pixel, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
            mode = LEGOLEDMatrix.Mode.Pixel;
            matrix.RegisterDelegate(this);
            IsConnected = true;
            Debug.Log(name + " connected");
            return true;
        }

        private void OnDestroy()
        {
            matrix?.UnregisterDelegate(this);
        }

        public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
        {
            if (newState == ServiceState.Disconnected)
            {
                Debug.LogWarning(name + " disconnected");
                matrix.UnregisterDelegate(this);
                matrix = null;
                IsConnected = false;
            }
        }
        #endregion
    }
}