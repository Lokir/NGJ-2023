// Copyright (C) LEGO System A/S - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited

using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace LEGOWirelessSDK
{
    public static class AndroidPermissionsHandler
    {
        static void StartLocationService()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Including Input.location.Start in code will prompt Unity to add location permissions to the Android manifest.
            Input.location.Start();
#endif
        }

        /// <summary>
        /// Requests and prompts user permissions for location services.
        /// </summary>
        public static void RequestUserLocationPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (GetAndroidSDKVersion() < 29)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            {
                Permission.RequestUserPermission(Permission.CoarseLocation);
            }
        }
        else if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif
        }

        /// <summary>
        /// Check if user has authorized permission.
        /// </summary>
        public static bool HasUserAuthorizedLocationPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (GetAndroidSDKVersion() < 29)
        {
            return Permission.HasUserAuthorizedPermission(Permission.CoarseLocation);
        }
        else
        {
            return Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        }
#else
            return true;
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    static int GetAndroidSDKVersion()
    {
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }
#endif
    }
}