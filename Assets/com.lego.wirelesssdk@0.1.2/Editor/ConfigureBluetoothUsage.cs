// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

#if UNITY_IOS
using System.IO;
#endif
using UnityEditor;
#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

namespace LEGOWirelessSDK
{
    public static class ConfigureBluetoothUsage
    {
        [InitializeOnLoadMethod]
        static void AddBluetoothForMacOS()
        {
            // Check if Bluetooth usage description has been set.
            if (string.IsNullOrEmpty(PlayerSettings.macOS.bluetoothUsageDescription))
            {
                PlayerSettings.macOS.bluetoothUsageDescription = "Required for connecting to LEGO device.";
            }
        }

#if UNITY_IOS
        [PostProcessBuild]
        public static void AddBluetoothForIOS(BuildTarget buildTarget, string pathToBuiltProject)
        {
            // Get plist.
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root.
            PlistElementDict rootDict = plist.root;

            // Set Bluetooth usage description.
            if (rootDict["NSBluetoothAlwaysUsageDescription"] == null)
            {
                rootDict.SetString("NSBluetoothAlwaysUsageDescription", "Required for connecting to LEGO device.");
            }

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
#endif
    }
}