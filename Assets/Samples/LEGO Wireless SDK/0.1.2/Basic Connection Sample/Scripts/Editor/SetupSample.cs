// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.Rendering;

namespace LEGOWirelessSDK.BasicConnectionSample
{
    public class SetupSample
    {
        static readonly string installedPipelinePath = "Assets/Samples/LEGO Wireless SDK/{0}/Basic Connection Sample/Rendering/UniversalRenderPipelineAsset.asset";
        static readonly string pipelineNoPromptPrefsKey = "com.lego.wirelesssdk.noPromptForIncorrectSampleRenderPipeline";

        [InitializeOnLoadMethod]
        static void DoSetupSample()
        {
            // Do not perform the check when playing.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            // Retrieve package version.
            var packageVersion = "Unknown";
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.lego.wirelesssdk");
            if (packageInfo != null)
            {
                packageVersion = packageInfo.version; 
            }

            // Determine if correct pipeline is set.
            var existingRenderPipeline = GraphicsSettings.defaultRenderPipeline;
            var sampleRenderPipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(string.Format(installedPipelinePath, packageVersion));

            var noPrompt = EditorPrefs.GetBool(pipelineNoPromptPrefsKey, false);

            if (!existingRenderPipeline && sampleRenderPipeline && !noPrompt)
            {
                EditorApplication.delayCall += () =>
                {
                    // Delay once more to give LEGO materials package a chance to set the pipeline.
                    EditorApplication.delayCall += () =>
                    {
                        // Check if LEGO materials 
                        var existingRenderPipeline = GraphicsSettings.defaultRenderPipeline;

                        // Prompt the user to enable the sample render pipeline.
                        if (!existingRenderPipeline)
                        {
                            var answer = EditorUtility.DisplayDialogComplex("Universal Render Pipeline required by Basic Connection Sample", "Do you want to enable Universal Render Pipeline?", "Yes", "No", "No, Don't Show Again");
                            switch (answer)
                            {
                                // Yes
                                case 0:
                                    {
                                        GraphicsSettings.defaultRenderPipeline = sampleRenderPipeline;
                                        break;
                                    }
                                // No, Don't Show Again
                                case 2:
                                    {
                                        EditorPrefs.SetBool(pipelineNoPromptPrefsKey, true);
                                        break;
                                    }
                            }
                        }
                    };
                };
            }
        }
    }
}

#endif