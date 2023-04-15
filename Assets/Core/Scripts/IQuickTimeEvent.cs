using System;

namespace Core.Scripts
{
    public interface IQuickTimeEventDependencies
    {
        Action<IQuickTimeEventPayload> CompleteEvent { get; }
    }
    public interface IQuickTimeEventPayload
    {
        bool Success { get; }
    }
    public interface IQuickTimeEvent
    {
        void Initialize(IQuickTimeEventDependencies quicktimeDependencies);
        void PlayEvent();
        void StopEvent();
    }
}
