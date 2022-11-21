using UnityEngine;

namespace Crysc.Registries
{
    public interface IRegistrar<out T> where T : Object
    {
        public T Registrant { get; }
    }
}
