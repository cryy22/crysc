using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Registries
{
    public abstract class Registry<T> : ScriptableObject
        where T : Component
    {
        [NonSerialized] protected readonly HashSet<IRegistrar<T>> Registrars = new();

        public IEnumerable<T> Members => Registrars.Select(r => r.Registrant);

        protected virtual void OnEnable()
        {
            foreach (IRegistrar<T> registrar in Registrars) SubscribeToEvents(registrar);
        }

        protected virtual void OnDisable()
        {
            foreach (IRegistrar<T> registrar in Registrars) UnsubscribeFromEvents(registrar);
        }

        public void Register(IRegistrar<T> registrar)
        {
            Registrars.Add(registrar);
            SubscribeToEvents(registrar);
        }

        public void Unregister(IRegistrar<T> registrar)
        {
            UnsubscribeFromEvents(registrar);
            Registrars.Remove(registrar);
        }

        protected virtual void SubscribeToEvents(IRegistrar<T> registrar) { }
        protected virtual void UnsubscribeFromEvents(IRegistrar<T> registrar) { }
    }
}
