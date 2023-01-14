using System;
using UnityEngine;

namespace Crysc.Patterns.Registries
{
    public interface IMouseEventRegistrar<T> : IRegistrar<T>
    {
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;

        public Bounds Bounds { get; }
    }
}
