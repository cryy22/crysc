using System;
using UnityEngine;

namespace Crysc.Patterns.Registries
{
    public abstract class Registrar<T> : MonoBehaviour, IRegistrar<T>
    {
        public virtual T Registrant { get; private set; }
        protected abstract Registry<T> Registry { get; }

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
