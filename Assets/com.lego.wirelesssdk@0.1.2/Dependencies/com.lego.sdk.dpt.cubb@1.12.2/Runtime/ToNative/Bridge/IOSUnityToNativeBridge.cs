using System.Runtime.InteropServices;
using AOT;
using CoreUnityBleBridge.ToUnity;

namespace CoreUnityBleBridge.ToNative.Bridge
{
	internal sealed class IOSUnityToNativeBridge : AbstractUnityToNativeBridge
	{
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
        private static extern void _cubbHandleMessageFromUnity(string message);
		
		[DllImport("__Internal")]
		public static extern void setCallback(CubbSendMessageDelegate callback);
#else
		private static void _cubbHandleMessageFromUnity(string message) { }

		public static void setCallback(CubbSendMessageDelegate callback) { }
#endif
	    
		public delegate void CubbSendMessageDelegate(string message);
 

		public IOSUnityToNativeBridge()
		{
			setCallback(ReceiveMessageFromNative);
		}
	    
		[MonoPInvokeCallback(typeof(CubbSendMessageDelegate))]
		private static void ReceiveMessageFromNative(string message)
		{
			CUBBNativeMessageReceiver.ReceiveMessage(message);
		}

		protected override void SendMessageToNative(string message)
		{
			// Extern method which sends a message to native
			_cubbHandleMessageFromUnity(message);
		}
	}
}