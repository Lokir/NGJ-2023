// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LEGOWirelessSDK
{
    public class SoundPlayer : ServiceBase
    {
        public void PlayTone(SoundPlayerTone tone)
        {
            if (soundPlayer == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            if (mode != LEGOSoundPlayer.LESoundPlayerMode.Tone)
            {
                soundPlayer.UpdateInputFormat(new LEGOInputFormat(soundPlayer.ConnectInfo.PortID, soundPlayer.ioType, (int)LEGOSoundPlayer.LESoundPlayerMode.Tone, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
                mode = LEGOSoundPlayer.LESoundPlayerMode.Tone;
            }
            soundPlayer.SendCommand(new LEGOSoundPlayer.PlayToneIndexCommand() { Value = (LEGOSoundPlayer.LESoundPlayerTone)tone });
        }

        public void PlaySound(SoundPlayerSound sound)
        {
            if (soundPlayer == null)
            {
                Debug.LogError(name + " is not connected");
                return;
            }
            if (mode != LEGOSoundPlayer.LESoundPlayerMode.Sound)
            {
                soundPlayer.UpdateInputFormat(new LEGOInputFormat(soundPlayer.ConnectInfo.PortID, soundPlayer.ioType, (int)LEGOSoundPlayer.LESoundPlayerMode.Sound, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
                mode = LEGOSoundPlayer.LESoundPlayerMode.Sound;
            }
            soundPlayer.SendCommand(new LEGOSoundPlayer.PlaySoundIndexCommand() { Value = (LEGOSoundPlayer.LESoundPlayerSound)sound });
        }

        #region internals
        public enum SoundPlayerTone
        {
            //Stop = 0,
            Low = 3,
            Medium = 9,
            High = 10
        }

        public enum SoundPlayerSound
        {
            Stop = 0,
            Sound1 = 3,
            Sound2 = 5,
            Sound3 = 7,
            Sound4 = 9,
            Sound5 = 10
        }

        private LEGOSoundPlayer soundPlayer;

        private LEGOSoundPlayer.LESoundPlayerMode mode = LEGOSoundPlayer.LESoundPlayerMode.Tone;

        public override bool Setup(ICollection<ILEGOService> services)
        {
            if (IsConnected)
            {
                return true;
            }
            soundPlayer = services.FirstOrDefault(s => s.ioType == IOType.LEIOTypeSoundPlayer) as LEGOSoundPlayer;
            if (soundPlayer == null)
            {
                Debug.LogWarning(name + " service not found");
                return false;
            }
            services.Remove(soundPlayer);
            soundPlayer.UpdateInputFormat(new LEGOInputFormat(soundPlayer.ConnectInfo.PortID, soundPlayer.ioType, (int)LEGOSoundPlayer.LESoundPlayerMode.Tone, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
            mode = LEGOSoundPlayer.LESoundPlayerMode.Tone;
            soundPlayer.RegisterDelegate(this);
            IsConnected = true;
            Debug.Log(name + " connected");
            return true;
        }

        private void OnDestroy()
        {
            soundPlayer?.UnregisterDelegate(this);
        }

        public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
        {
            if (newState == ServiceState.Disconnected)
            {
                Debug.LogWarning(name + " disconnected");
                soundPlayer.UnregisterDelegate(this);
                soundPlayer = null;
                IsConnected = false;
            }
        }
        #endregion
    }
}