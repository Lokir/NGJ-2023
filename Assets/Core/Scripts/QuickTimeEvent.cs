using System;
using UnityEngine;

namespace Core.Scripts
{
    public class QuickTimeEvent : MonoBehaviour, IQuickTimeEvent
    {
        public virtual void Initialize(IQuickTimeEventDependencies quicktimeDependencies)
        {
            throw new ArgumentException("Not designed to use base implementation.");
        }

        public virtual void PlayEvent()
        {
            throw new ArgumentException("Not designed to use base implementation.");
        }

        public virtual void StopEvent()
        {
            throw new ArgumentException("Not designed to use base implementation.");
        }
    }
}
