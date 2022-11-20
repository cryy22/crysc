using System;
using UnityEngine;

namespace Crysc.Registries
{
    public interface IRegistrar<T> where T : Component
    {
        public event EventHandler<RegistryEventArgs<T>> Destroying;
        public T Registrant { get; }
    }
}
