using System;

namespace Crysc.Registries
{
    public interface ILifecycleRegistrar<T> : IRegistrar<T>
    {
        public event EventHandler<RegistryEventArgs<T>> Destroying;
    }
}
