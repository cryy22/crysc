using UnityEngine;

namespace Crysc.Registries
{
    public abstract class Registrar<T> : MonoBehaviour, IRegistrar<T> where T : Component
    {
        [SerializeField] protected Registry<T> Registry;

        protected virtual void Awake() { Registrant = GetComponent<T>(); }
        protected virtual void Start() { Registry.Register(this); }
        protected virtual void OnDestroy() { Registry.Unregister(this); }

        protected RegistryEventArgs<T> BuildEventArgs()
        {
            return new RegistryEventArgs<T>(registrar: this, registrant: Registrant);
        }

        // IRegistrar
        public T Registrant { get; private set; }
    }
}
