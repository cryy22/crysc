using System;

namespace Crysc.Patterns.Coordination
{
    public class ManagerEventArgs : EventArgs
    {
        public bool IsActive { get; }
        public ManagerEventArgs(bool isActive) { IsActive = isActive; }
    }
}
