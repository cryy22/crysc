using System;
using UnityEngine;

namespace Crysc.Registries
{
    public class RegistryEventArgs<T> : EventArgs where T : Component
    {
        public RegistryEventArgs(T registrant, IRegistrar<T> registrar)
        {
            Registrant = registrant;
            Registrar = registrar;
        }

        public T Registrant { get; }
        public IRegistrar<T> Registrar { get; }
    }
}
