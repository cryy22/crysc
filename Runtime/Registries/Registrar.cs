using System;
using UnityEngine;

namespace Crysc.Registries
{
    public abstract class Registrar<T> : MonoBehaviour, IRegistrar<T> where T : Component
    {
        [SerializeField] protected Registry<T> Registry;

        protected virtual void Awake() { Registrant = GetComponent<T>(); }
        protected virtual void Start() { Registry.Register(this); }

        protected virtual void OnDestroy()
        {
            Destroying?.Invoke(sender: Registrant, e: EventArgs.Empty);
            Registry.Unregister(this);
        }

        // IRegistrar
        public event EventHandler Destroying;
        public T Registrant { get; private set; }
    }
}
