// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using TMPro;
using UnityEngine;

namespace LEGOWirelessSDK.BasicConnectionSample
{
    public class TachoMotorController : MonoBehaviour
    {
        [Header("Hardware")]
        [SerializeField] HubBase hub;
        [SerializeField] TachoMotor tachoMotor;

        [Header("UI")]
        [SerializeField] TMP_Text output;
        [SerializeField] Renderer uiHub;
        [SerializeField] Transform uiMotorHead;

        public void OnIsConnectedChanged(bool connected)
        {
            // Show connection status.
            output.text = connected ? "Connected" : "Not connected";
        }

        public void OnPositionChanged(int position)
        {
            // Show current position.
            output.text = "Position " + position;

            // Rotate motor head to reflect the position.
            uiMotorHead.localRotation = Quaternion.Euler(0f, position, 0f);

            // Set the hub light to reflect the position.
            var color = Color.HSVToRGB((position + 180f) / 360f, 1f, 1f);
            hub.LedColor = color;

            // ...and also set the ui hub light.
            uiHub.materials[2].color = color * 4f;
            uiHub.materials[2].SetColor("_EmissionColor", color * 4f);
            uiHub.materials[2].EnableKeyword("_EMISSION");
        }
    }
}
