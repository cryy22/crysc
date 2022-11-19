using System;

namespace Crysc.Registries
{
    public interface IRegisterable
    {
        public event EventHandler Destroying;
    }
}
