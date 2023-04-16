using System;
using UnityEngine;

namespace Helpers
{
    public class TimerFixedUpdateLoop : MonoBehaviour, ITimer
    {
        public void Awake()
        {
            this.enabled = false;
        }

        public float TargetTime { get; private set; }
        public float TimeElapsed { get; private set; }
        public bool IsRunning => this.enabled;
        private Action<float> step;
        private Action completed;
        public void StartTimer(float targetTime, Action<float> step = null, Action completed = null)
        {
            this.TargetTime = targetTime;
            this.step = step;
            this.completed = completed;
            TimeElapsed = 0f;
            this.enabled = true;
        }

        public void StopTimer()
        {
            this.enabled = false;
            completed = null;
            step = null;
        }

        public void PauseTimer()
        {
            this.enabled = false;
        }

        public void UnpauseTimer()
        {
            this.enabled = true;
        }

        private void FixedUpdate()
        {
            if (TimeElapsed > TargetTime)
            {
                CompleteTimer();
                return;
            }
            TimeElapsed += Time.fixedDeltaTime;
            step?.Invoke(Time.fixedDeltaTime);
        }

        private void CompleteTimer()
        {
            completed?.Invoke();
            StopTimer();
        }
    }
}
