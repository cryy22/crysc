using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Crysc.Registries
{
    public interface IMouseEventRegistrar<T> : IRegistrar<T> where T : Object
    {
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;

        public Bounds Bounds { get; }
    }
}
