using System;
using Object = UnityEngine.Object;

namespace Crysc.Registries
{
    public interface ILifecycleRegistrar<T> : IRegistrar<T> where T : Object
    {
        public event EventHandler<RegistryEventArgs<T>> Destroying;
    }
}
