using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Registries
{
    public abstract class Registry<T> : ScriptableObject where T : Component
    {
        [NonSerialized] private readonly HashSet<Registrar<T>> _registrars = new();

        public IEnumerable<T> Members => _registrars.Select(r => r.Registrant);

        private void OnEnable()
        {
            foreach (T member in Members) SubscribeToEvents(member);
        }

        private void OnDisable()
        {
            foreach (T member in Members) UnsubscribeFromEvents(member);
        }

        public void Register(Registrar<T> registrar)
        {
            _registrars.Add(registrar);
            SubscribeToEvents(registrar.Registrant);
        }

        public void Unregister(Registrar<T> registrar)
        {
            UnsubscribeFromEvents(registrar.Registrant);
            _registrars.Remove(registrar);
        }

        protected virtual void SubscribeToEvents(T member) { }
        protected virtual void UnsubscribeFromEvents(T member) { }
    }
}
