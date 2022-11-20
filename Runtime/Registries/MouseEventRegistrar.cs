using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Registries
{
    [RequireComponent(typeof(Collider))]
    public abstract class MouseEventRegistrar<T> : Registrar<T>, IMouseEventRegistrar<T>,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        where T : Component
    {
        private void OnDisable() { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }

        // IMouseEventRegistrar
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;

        // IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerEnter(PointerEventData _) { Hovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerExit(PointerEventData _) { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
    }
}
