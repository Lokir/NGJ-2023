#if UNITY_EDITOR
using LEGOModelImporter;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class InitializeProject
{
    internal static readonly string initializedMarkerPath = "InitializedMarker";

    internal static readonly string scenesPath = "Assets/Samples/LEGO Wireless SDK/{0}/Basic Connection Sample/Scenes";
    internal static readonly string sampleSceneName = "Sample Scene.unity";

    [MenuItem("LEGO Tools/Open Connection Sample", false, priority = 201)]
    static void LoadConnectionSampleScene()
    {
        // Retrieve package version.
        var packageVersion = "Unknown";
        var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.lego.wirelesssdk");
        if (packageInfo != null)
        {
            packageVersion = packageInfo.version;
        }


        // Load sample scene.
        var sampleScenePath = Path.Combine(string.Format(scenesPath, packageVersion), sampleSceneName);
        var sampleScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sampleScenePath);
        if (sampleScene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(sampleScenePath);
            }
        }
    }

    [InitializeOnLoadMethod]
    static void Initialize()
    {
        if (!File.Exists(initializedMarkerPath))
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                File.CreateText(initializedMarkerPath).Close();

                EditorApplication.delayCall += LoadConnectionSampleScene;
            }
        }
    }
}
#endif
