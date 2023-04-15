// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using TMPro;
using UnityEngine;

namespace LEGOWirelessSDK.BasicConnectionSample
{
    public class TechnicHubController : MonoBehaviour
    {
        [Header("Hardware")]
        [SerializeField] HubBase hub;

        [Header("UI")]
        [SerializeField] TMP_Text output;
        [SerializeField] Transform uiHub;
        [SerializeField] Transform uiHubButton;
        [SerializeField] ParticleSystem uiHubButtonEffect;
        [SerializeField] Transform xAxisPositive;
        [SerializeField] Transform xAxisNegative;
        [SerializeField] Transform yAxisPositive;
        [SerializeField] Transform yAxisNegative;
        [SerializeField] Transform zAxisPositive;
        [SerializeField] Transform zAxisNegative;

        public void OnIsConnectedChanged(bool connected)
        {
            // Show connection status.
            output.text = connected ? "Connected" : "Disconnected";
        }

        public void OnButtonChanged(bool pressed)
        {
            // Show effect when button is pressed.
            uiHubButton.localPosition = new Vector3(0f, pressed ? 1.9f : 2f, 1.6f);
            if (pressed)
            {
                uiHubButtonEffect.Emit(20);
            }
        }

        public void OnAccelerationChanged(Vector3 acceleration)
        {
            // Show hub acceleration in ui.
            xAxisPositive.gameObject.SetActive(acceleration.x > 0.01f);
            xAxisNegative.gameObject.SetActive(acceleration.x < -0.01f);
            xAxisPositive.localScale = new Vector3(1f, 1f, 10f * Mathf.Max(0f, acceleration.x));
            xAxisNegative.localScale = new Vector3(1f, 1f, 10f * Mathf.Min(0f, acceleration.x));

            yAxisPositive.gameObject.SetActive(acceleration.y > 0.01f);
            yAxisNegative.gameObject.SetActive(acceleration.y < -0.01f);
            yAxisPositive.localScale = new Vector3(1f, 1f, 10f * Mathf.Max(0f, acceleration.y));
            yAxisNegative.localScale = new Vector3(1f, 1f, 10f * Mathf.Min(0f, acceleration.y));

            zAxisPositive.gameObject.SetActive(acceleration.z > 0.01f);
            zAxisNegative.gameObject.SetActive(acceleration.z < -0.01f);
            zAxisPositive.localScale = new Vector3(1f, 1f, 10f * Mathf.Max(0f, acceleration.z));
            zAxisNegative.localScale = new Vector3(1f, 1f, 10f * Mathf.Min(0f, acceleration.z));
        }

        public void OnOrientationChanged(Vector3 orientation)
        {
            // Rotate the ui hub.
            uiHub.rotation = Quaternion.Euler(orientation);
        }

        void Update()
        {
            // Show battery level.
            if (hub.IsConnected)
            {
                output.text = "Battery " + hub.BatteryLevel + "%";
            }
        }
    }
}
