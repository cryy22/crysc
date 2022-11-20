using System;
using Crysc.Registries;
using UnityEngine;

namespace Crysc.UI.Tooltip
{
    public interface ITooltipRegistrar<out T> : IRegistrar<T> where T : Component
    {
        public event EventHandler Hovered;
        public event EventHandler Unhovered;
    }
}
