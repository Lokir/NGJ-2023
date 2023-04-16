using System;
using UnityEngine;

namespace CoreUnityBleBridge.ToNative.Bridge
{
    internal class MikeMainThreadDispatcher : MonoBehaviour
    {
        private static Action _queue;
        private static Action _destroyQueue;
        private static object _lockObject = new object();
        private static MikeMainThreadDispatcher _instance;

        public static Action onUpdate;

        private void Update()
        {
            lock (_lockObject)
            {
                _queue?.Invoke();
                _queue = null;
            }
            onUpdate?.Invoke();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("MikeMainThreadDispatcher OnApplicationQuit");
            _destroyQueue?.Invoke();
        }

        public static void Enqueue(Action action)
        {
            lock (_lockObject)
            {
                _queue += action;
            }
        }

        public static void EnqueueOnDestroy(Action action)
        {
            lock (_lockObject)
            {
                _destroyQueue += action;
            }
        }

        public static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new GameObject("Mike MainThreadDispatcher").AddComponent<MikeMainThreadDispatcher>();
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }
}
