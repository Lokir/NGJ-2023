#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using System;
using System.Runtime.InteropServices;
using AOT;
using CoreUnityBleBridge.ToUnity;

namespace CoreUnityBleBridge.ToNative.Bridge
{
    internal sealed class OSXUnityToNativeBridge : AbstractUnityToNativeBridge
    {
        private delegate void UnityMessageDelegate(IntPtr objectName, IntPtr commandName, IntPtr commandData);

        [DllImport ("CUBBNativeOSX")]
        private static extern void _cubbHandleMessageFromUnity(string message);
        [DllImport ("CUBBNativeOSX")]
        private static extern void LEGODeviceSDK_setOSXUnityMessageCallback(UnityMessageDelegate messageCallback);
        
        static OSXUnityToNativeBridge()
        {
            SetCallback();
        }

        ~OSXUnityToNativeBridge()
        {
	        LEGODeviceSDK_setOSXUnityMessageCallback(null);
        }
        
        public static void SetCallback()
        {
            // Hook up the message callback
            LEGODeviceSDK_setOSXUnityMessageCallback(MessageCallback );

        }
        
        [MonoPInvokeCallback(typeof(UnityMessageDelegate))]
        private static void MessageCallback(IntPtr objectname, IntPtr commandname, IntPtr commanddata)
        {
            CUBBNativeMessageReceiver.ReceiveMessage(Marshal.PtrToStringAuto(commanddata));
        }

        protected override void SendMessageToNative(string message)
        {
            _cubbHandleMessageFromUnity(message);
        }
    }
}
#endif