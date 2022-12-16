using System;

namespace Crysc.Coordination
{
    public class CoordinationEventArgs : EventArgs
    {
        public CoordinationEventArgs(bool isActive) { IsActive = isActive; }
        public bool IsActive { get; }
    }
}
