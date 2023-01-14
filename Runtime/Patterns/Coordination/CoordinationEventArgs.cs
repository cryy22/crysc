using System;

namespace Crysc.Patterns.Coordination
{
    public class CoordinationEventArgs : EventArgs
    {
        public CoordinationEventArgs(bool isActive) { IsActive = isActive; }
        public bool IsActive { get; }
    }
}
