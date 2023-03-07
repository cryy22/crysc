using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Patterns.Registries
{
    public abstract class Registry<T> : ScriptableObject
    {
        [NonSerialized] private readonly HashSet<IRegistrar<T>> _registrars = new();

        protected IEnumerable<T> Members => _registrars.Select(r => r.Registrant);

        public event EventHandler<RegistryEventArgs<T>> Destroying;
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;

        protected virtual void OnEnable()
        {
            foreach (IRegistrar<T> registrar in _registrars) SubscribeToEvents(registrar);
        }

        protected virtual void OnDisable()
        {
            foreach (IRegistrar<T> registrar in _registrars) UnsubscribeFromEvents(registrar);
        }

        public void Register(IRegistrar<T> registrar)
        {
            _registrars.Add(registrar);
            SubscribeToEvents(registrar);
        }

        public void Unregister(IRegistrar<T> registrar)
        {
            UnsubscribeFromEvents(registrar);
            _registrars.Remove(registrar);
        }

        protected virtual void SubscribeToEvents(IRegistrar<T> registrar)
        {
            if (registrar is ILifecycleRegistrar<T> lcRegistrar)
                lcRegistrar.Destroying += DestroyingEventHandler;
            if (registrar is IMouseEventRegistrar<T> meRegistrar)
            {
                meRegistrar.Hovered += HoveredEventHandler;
                meRegistrar.Unhovered += UnhoveredEventHandler;
                meRegistrar.Clicked += ClickedEventHandler;
            }
        }

        protected virtual void UnsubscribeFromEvents(IRegistrar<T> registrar)
        {
            if (registrar is ILifecycleRegistrar<T> lcRegistrar)
                lcRegistrar.Destroying -= DestroyingEventHandler;
            if (registrar is IMouseEventRegistrar<T> meRegistrar)
            {
                meRegistrar.Hovered -= HoveredEventHandler;
                meRegistrar.Unhovered -= UnhoveredEventHandler;
                meRegistrar.Clicked -= ClickedEventHandler;
            }
        }

        private void HoveredEventHandler(object sender, RegistryEventArgs<T> e)
        {
            Hovered?.Invoke(sender: sender, e: e);
        }

        private void UnhoveredEventHandler(object sender, RegistryEventArgs<T> e)
        {
            Unhovered?.Invoke(sender: sender, e: e);
        }

        private void ClickedEventHandler(object sender, RegistryEventArgs<T> e)
        {
            Clicked?.Invoke(sender: sender, e: e);
        }

        private void DestroyingEventHandler(object sender, RegistryEventArgs<T> e)
        {
            Destroying?.Invoke(sender: sender, e: e);
        }
    }
}
