using UnityEngine;

namespace Crysc.Registries
{
    public interface IRegistrar<out T> where T : Component
    {
        public T Registrant { get; }
    }
}
