using System;

namespace Crysc.Patterns.Managers
{
    public class ManagerEventArgs : EventArgs
    {
        public bool IsActive { get; }
        public ManagerEventArgs(bool isActive) { IsActive = isActive; }
    }
}
