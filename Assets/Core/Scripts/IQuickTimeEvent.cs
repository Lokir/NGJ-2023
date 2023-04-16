using System;
using FMODUnity;

namespace Core.Scripts
{
    public interface IQuickTimeEventDependencies
    {
        QuicktimeShower Shower { get; }
        StudioEventEmitter LossSoundEmitter { get; }
        StudioEventEmitter WinSoundEmitter { get; }
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
