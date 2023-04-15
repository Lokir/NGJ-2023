using System;

namespace Helpers
{
    public interface ITimer
    {
        float TargetTime { get; }
        float TimeElapsed { get; }
        public void StartTimer(float availableTime, Action<float> step = null, Action completed = null);
        void StopTimer();
        void PauseTimer();
        void UnpauseTimer();
    }
}