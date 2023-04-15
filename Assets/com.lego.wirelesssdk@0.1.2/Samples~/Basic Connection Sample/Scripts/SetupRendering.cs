// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LEGOWirelessSDK
{

    public class SetupRendering : MonoBehaviour
    {
        [SerializeField] VolumeProfile volumeProfile;
        [SerializeField] RenderPipelineAsset renderPipelineAsset;

        void Awake()
        {
            // Adjust bloom settings according to active colour space.
            if (volumeProfile.TryGet<Bloom>(out var bloom))
            {
                if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                {
                    bloom.intensity.value = 0.1f;
                }
                else
                {
                    bloom.intensity.value = 1.0f;
                }
            }

            // Set the correct pipeline.
            GraphicsSettings.defaultRenderPipeline = renderPipelineAsset;
            QualitySettings.renderPipeline = renderPipelineAsset;
        }
    }
}