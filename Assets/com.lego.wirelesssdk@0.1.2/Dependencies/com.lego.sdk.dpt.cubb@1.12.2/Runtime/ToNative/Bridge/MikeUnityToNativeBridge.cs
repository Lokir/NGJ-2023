using System.Runtime.InteropServices;
using CoreUnityBleBridge.ToUnity;
using AOT;

namespace CoreUnityBleBridge.ToNative.Bridge
{
	internal sealed class MikeUnityToNativeBridge : AbstractUnityToNativeBridge
	{
		public delegate void CubbSendMessageDelegate(string message);

        [DllImport("winrt1", CallingConvention = CallingConvention.Cdecl)]
        private static extern void _cubbHandleMessageFromUnity(string message);

        [DllImport("winrt1", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setCallback(CubbSendMessageDelegate callback);

        public MikeUnityToNativeBridge()
		{
			MikeMainThreadDispatcher.Initialize();
			MikeMainThreadDispatcher.onUpdate += () => _cubbHandleMessageFromUnity("Poll");
			MikeMainThreadDispatcher.EnqueueOnDestroy(() => {
				UnityEngine.Debug.Log("unsetting callback"); // todo still needed?
				setCallback(null);
			});
			setCallback(ReceiveMessageFromNative);
		}

		[MonoPInvokeCallback(typeof(CubbSendMessageDelegate))]
		private static void ReceiveMessageFromNative(string message)
		{
			// switched to "pull" so this runs on unity thread, call directly
			CUBBNativeMessageReceiver.ReceiveMessage(message);
			//MikeMainThreadDispatcher.Enqueue(() => CUBBNativeMessageReceiver.ReceiveMessage(message));
		}

        protected override void SendMessageToNative(string message)
        {
            _cubbHandleMessageFromUnity(message);
        }

    }
}
