using System;

namespace Crysc.Patterns.Registries
{
    public interface ILifecycleRegistrar<T> : IRegistrar<T>
    {
        public event EventHandler<RegistryEventArgs<T>> Destroying;
    }
}
