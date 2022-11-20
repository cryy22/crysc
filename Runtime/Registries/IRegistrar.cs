using System;
using UnityEngine;

namespace Crysc.Registries
{
    public interface IRegistrar<out T> where T : Component
    {
        public event EventHandler Destroying;
        public T Registrant { get; }
    }
}
