using System;
using UnityEngine;

namespace Crysc.Registries
{
    public interface IMouseEventRegistrar<out T> : IRegistrar<T> where T : Component
    {
        public event EventHandler Hovered;
        public event EventHandler Unhovered;
        public event EventHandler Clicked;
    }
}
