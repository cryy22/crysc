using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Registries
{
    public abstract class Registry<T> : ScriptableObject
        where T : Component
    {
        [NonSerialized] private readonly HashSet<IRegistrar<T>> Registrars = new();

        public event EventHandler Hovered;
        public event EventHandler Unhovered;
        public event EventHandler Clicked;

        protected IEnumerable<T> Members => Registrars.Select(r => r.Registrant);

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

        protected virtual void SubscribeToEvents(IRegistrar<T> registrar)
        {
            if (registrar is IMouseEventRegistrar<T> meRegistrar)
            {
                meRegistrar.Hovered += HoveredEventHandler;
                meRegistrar.Unhovered += UnhoveredEventHandler;
                meRegistrar.Clicked += ClickedEventHandler;
            }
        }

        protected virtual void UnsubscribeFromEvents(IRegistrar<T> registrar)
        {
            if (registrar is IMouseEventRegistrar<T> meRegistrar)
            {
                meRegistrar.Hovered -= HoveredEventHandler;
                meRegistrar.Unhovered -= UnhoveredEventHandler;
                meRegistrar.Clicked -= ClickedEventHandler;
            }
        }

        private void HoveredEventHandler(object sender, EventArgs e) { Hovered?.Invoke(sender: sender, e: e); }
        private void UnhoveredEventHandler(object sender, EventArgs e) { Unhovered?.Invoke(sender: sender, e: e); }
        private void ClickedEventHandler(object sender, EventArgs e) { Clicked?.Invoke(sender: sender, e: e); }
    }
}
