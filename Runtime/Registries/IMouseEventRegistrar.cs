using System;
using UnityEngine;

namespace Crysc.Registries
{
    public interface IMouseEventRegistrar<T> : IRegistrar<T> where T : Component
    {
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;
    }
}
