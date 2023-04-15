// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using System.Collections;
using TMPro;
using UnityEngine;

namespace LEGOWirelessSDK.BasicConnectionSample
{
    public class DistanceSensorController : MonoBehaviour
    {
        [Header("Hardware")]
        [SerializeField] DistanceSensor distanceSensor;

        [Header("UI")]
        [SerializeField] TMP_Text output;
        [SerializeField] Renderer uiDistanceSensorUpperLeftLight;
        [SerializeField] Renderer uiDistanceSensorUpperRightLight;
        [SerializeField] Renderer uiDistanceSensorLowerLeftLight;
        [SerializeField] Renderer uiDistanceSensorLowerRightLight;

        private Color initialColor;

        private void Awake()
        {
            initialColor = uiDistanceSensorUpperLeftLight.materials[0].color;

            StartCoroutine(DoAnimate());
        }

        private IEnumerator DoAnimate()
        {
            while (true)
            {
                // Look.
                UpdateIntensities(100, 100, 100, 100);

                yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

                // Wink.
                UpdateIntensities(0, 100, 25, 100);

                yield return new WaitForSeconds(Random.Range(0.3f, 0.4f));
            }
        }

        private void UpdateIntensities(int upperLeft, int upperRight, int lowerLeft, int lowerRight)
        {
            if (distanceSensor.IsConnected)
            {
                // Set the intensity of the lights.
                distanceSensor.SetIntensity(upperLeft, upperRight, lowerLeft, lowerRight);
            }

            // ...and also set the ui light intensities.
            SetUIIntensity(uiDistanceSensorUpperLeftLight, upperLeft);
            SetUIIntensity(uiDistanceSensorUpperRightLight, upperRight);
            SetUIIntensity(uiDistanceSensorLowerLeftLight, lowerLeft);
            SetUIIntensity(uiDistanceSensorLowerRightLight, lowerRight);
        }

        private void SetUIIntensity(Renderer light, int intensity)
        {
            if (distanceSensor.IsConnected && intensity > 10)
            {
                var color = Color.HSVToRGB(0f, 0f, intensity / 100f);
                light.materials[0].color = color * 4f;
                light.materials[0].SetColor("_EmissionColor", color * 4f);
                light.materials[0].EnableKeyword("_EMISSION");
            }
            else
            {
                light.materials[0].color = initialColor;
                light.materials[0].DisableKeyword("_EMISSION");
            }
        }

        public void OnIsConnectedChanged(bool connected)
        {
            // Show connection status.
            output.text = connected ? "Connected" : "Not connected";
        }

        public void OnDistanceChanged(float distance)
        {
            // Show current distance.
            output.text = "Distance " + distance;
        }
    }
}
