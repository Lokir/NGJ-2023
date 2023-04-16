// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using TMPro;
using UnityEngine;

namespace LEGOWirelessSDK.BasicConnectionSample
{
    public class ForceSensorController : MonoBehaviour
    {
        [Header("Hardware")]
        [SerializeField] TachoMotor tachoMotor;
        [SerializeField] ForceSensor forceSensor;

        [Header("UI")]
        [SerializeField] TMP_Text output;
        [SerializeField] Transform uiForceSensorButton;

        private Color initialColor;

        public void OnIsConnectedChanged(bool connected)
        {
            // Show connection status.
            output.text = connected ? "Connected" : "Not connected";
        }

        public void OnForceChanged(int force)
        {
            // Show current force.
            output.text = "Force " + force;

            // Position the ui force sensor button.
            uiForceSensorButton.localPosition = new Vector3(0f, 0f, 2.8f - force * 0.8f / 100f);

            // Calculate normalised force.
            var normalisedForce = Mathf.RoundToInt(Mathf.Max(0f, (force - 25f) / 75f) * 100f);

            // Set motor power to reflect the force.
            if (tachoMotor.IsConnected)
            {
                tachoMotor.SetPower(normalisedForce);
            }
        }
    }
}
