using System;

namespace Crysc.Patterns.Registries
{
    public abstract class LifecycleRegistrar<T> : Registrar<T>, ILifecycleRegistrar<T>
    {
        // ILifecycleRegistrar
        public event EventHandler<RegistryEventArgs<T>> Destroying;

        protected override void OnDestroy()
        {
            Destroying?.Invoke(sender: Registrant, e: BuildEventArgs());
            base.OnDestroy();
        }
    }
}
