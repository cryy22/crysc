using System;
using UnityEngine;

namespace Crysc.Registries
{
    public interface ILifecycleRegistrar<T> : IRegistrar<T> where T : Component
    {
        public event EventHandler<RegistryEventArgs<T>> Destroying;
    }
}
