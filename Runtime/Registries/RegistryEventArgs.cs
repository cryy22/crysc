using System;
using Object = UnityEngine.Object;

namespace Crysc.Registries
{
    public class RegistryEventArgs<T> : EventArgs where T : Object
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
