using System;

namespace Crysc.Registries
{
    public class LifecycleRegistrar<T> : Registrar<T>, ILifecycleRegistrar<T>
    {
        protected override void OnDestroy()
        {
            Destroying?.Invoke(sender: Registrant, e: BuildEventArgs());
            base.OnDestroy();
        }

        // ILifecycleRegistrar
        public event EventHandler<RegistryEventArgs<T>> Destroying;
    }
}
