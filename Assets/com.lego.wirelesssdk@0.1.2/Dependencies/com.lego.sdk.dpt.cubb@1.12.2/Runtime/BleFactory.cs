using System;
using System.Diagnostics.CodeAnalysis;
using CoreUnityBleBridge.Model;
using CoreUnityBleBridge.ToNative;
using CoreUnityBleBridge.ToUnity;
using CoreUnityBleBridge.Verification;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge
{
    /// <summary>
    /// Public Core Unity Ble Bridge entry point.
    /// </summary>
    public static class BleFactory
    {
        private static bool isInitialized;
        private static BleBridge bleBridge;

        /// <summary>
        /// Initializes the Core Unity Bluetooth Low Energy Bridge Service. Can only be invoked once.
        /// </summary>
        /// <param name="bleSettings">The settings to utilize.</param>
        /// <param name="onInitialized">Invoked when the initialization is finished.</param>
        /// <exception cref="NotSupportedException">Thrown if an attemp to initialize the service while already initialized.</exception>
        public static void Initialize(BleSettings bleSettings, Action<InitializationEventArgs> onInitialized) 
        {
            if (isInitialized)
            {
                var message = "BleFactory: CUBB can only be initialized once. If BleSettings are different from first initialization, they will not be used. ";
                
                // Using Debug.LogWarning because AppManagers in LCC haven't initialized Loggers at this point
                // But logging with both since this might not always be the use case
                Debug.LogWarning(message);
                LogManager.GetLogger<BleSettings>().Warn(message);
                onInitialized?.Invoke(new InitializationEventArgs(bleBridge));
                bleBridge.EmitInitialEvents();
                return;
            }
                
            isInitialized = true;

            //Setup NativeToUnity communication
            var nativeToUnity = new NativeToUnity();
            var messageInterpreter = new NativeMessageInterpreter(nativeToUnity);
            var nativeMessageReceiver = CUBBNativeMessageReceiverFactory.Create();
            nativeMessageReceiver.MessageReceived += messageInterpreter.HandleMessage;

            // Setup native layer
            var unityToNative = UnityToNativeFactory.Create(nativeToUnity);

            // Inject checker, if called for
            BridgeChecker checker = null;
            if (bleSettings.EnableSelfCheck) 
            {
                checker = new BridgeChecker(nativeToUnity, unityToNative);
                nativeToUnity = checker.NativeToUnity;
                unityToNative = checker.UnityToNative;
            }

            // Setup Ble Bridge
            bleBridge = new BleBridge(nativeToUnity, unityToNative);
            if (checker != null)
                checker.BleBridge = bleBridge;
            
            // Initialize the native layer
            var gattServices = bleSettings.Filter.ToString();
            unityToNative.Initialize(gattServices);
            
            // Give client access to the bridge
            var args = new InitializationEventArgs(bleBridge);
            var action = onInitialized;
            if (action != null)
                action(args);
            
            // Emit initial bridge events
            //TODO: Ensure that any error events during initialization are emitted after "action" is invoked.
            bleBridge.EmitInitialEvents();
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    public sealed class InitializationEventArgs : EventArgs
    {
        public readonly IBleBridge BleBridge;

        public InitializationEventArgs(IBleBridge bleBridge)
        {
            BleBridge = bleBridge;
        }
    }
}