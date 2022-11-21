using System;
using Object = UnityEngine.Object;

namespace Crysc.Registries
{
    public class LifecycleRegistrar<T> : Registrar<T>, ILifecycleRegistrar<T> where T : Object
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
