using System;

namespace Crysc.Patterns.Registries
{
    public class RegistryEventArgs<T> : EventArgs
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
