// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LEGOWirelessSDK
{
    public class RGBLight
    {
        public void SetColor(Color color)
        {
            if (light == null)
            {
                Debug.LogError("rgb light service was not found during setup");
                return;
            }
            light.SendCommand(new LEGORGBLight.SetColorCommand() { Color = color * 255 });
        }

        #region internals
        private LEGORGBLight light;

        public bool Setup(ICollection<ILEGOService> services)
        {
            light = services.FirstOrDefault(s => s.ioType == IOType.LEIOTypeRGBLight) as LEGORGBLight;
            if (light == null)
            {
                Debug.LogWarning("rgb light service not found");
                return false;
            }
            services.Remove(light);
            light.UpdateInputFormat(new LEGOInputFormat(light.ConnectInfo.PortID, light.ioType, (int)LEGORGBLight.RGBLightMode.Absolute, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
            return true;
        }
        #endregion
    }
}