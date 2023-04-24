using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Patterns.Registries
{
    public abstract class Registrar<T> : MonoBehaviour, IRegistrar<T>
    {
        [FormerlySerializedAs("Registry")] [SerializeField] protected Registry<T> RegistryInput;

        public virtual T Registrant { get; private set; }

        protected virtual Registry<T> Registry => RegistryInput;

        protected virtual void Awake()
        {
            try
            {
                Registrant = GetComponentInParent<T>();
            }
            catch (ArgumentException)
            { }
        }

        protected virtual void Start() { Registry.Register(this); }
        protected virtual void OnDestroy() { Registry.Unregister(this); }

        protected RegistryEventArgs<T> BuildEventArgs()
        {
            return new RegistryEventArgs<T>(registrar: this, registrant: Registrant);
        }
    }
}
