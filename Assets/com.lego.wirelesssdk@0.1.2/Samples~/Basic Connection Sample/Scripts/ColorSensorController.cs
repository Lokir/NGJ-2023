// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LEGOWirelessSDK.BasicConnectionSample
{
    public class ColorSensorController : MonoBehaviour
    {
        [Header("Hardware")]
        [SerializeField] ColorSensorTechnic colorSensor;

        [Header("UI")]
        [SerializeField] TMP_Text output;
        [SerializeField] Renderer uiColorSensor;
        [SerializeField] Image colorSwatch;
        [SerializeField] Image colorSwatchNoColor;

        private Color initialColor;

        private void Awake()
        {
            initialColor = uiColorSensor.materials[2].color;
        }

        public void OnIsConnectedChanged(bool connected)
        {
            // Show connection status.
            output.text = connected ? "Connected" : "Not connected";

            // Set the ui color sensor light to reflect connection status.
            if (connected)
            {
                var color = Color.HSVToRGB(0f, 0f, 1f);
                uiColorSensor.materials[2].color = color * 4f;
                uiColorSensor.materials[2].SetColor("_EmissionColor", color * 4f);
                uiColorSensor.materials[2].EnableKeyword("_EMISSION");

                // Show color swatches.
                colorSwatch.gameObject.SetActive(colorSensor.Id > -1);
                colorSwatchNoColor.gameObject.SetActive(colorSensor.Id == 1);
            }
            else
            {
                uiColorSensor.materials[2].color = initialColor;
                uiColorSensor.materials[2].DisableKeyword("_EMISSION");

                // Hide color swatches.
                colorSwatch.gameObject.SetActive(false);
                colorSwatchNoColor.gameObject.SetActive(false);
            }
        }

        public void OnIdChanged(int id)
        {
            // Show current color.
            output.text = "Id " + id;
        }

        public void OnColorChanged(Color color)
        {
            // Update the color swatch to reflect the current color.
            colorSwatch.gameObject.SetActive(colorSensor.Id > -1);
            colorSwatch.color = color;

            // If we are not detecting a color, show the no-color swatch.
            colorSwatchNoColor.gameObject.SetActive(colorSensor.Id == -1);
        }
    }
}
