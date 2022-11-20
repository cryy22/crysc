using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Registries
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class MouseEventRegistrar<T> : Registrar<T>, IMouseEventRegistrar<T>,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        where T : Component
    {
        private Collider2D _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider2D>();
        }

        private void OnDisable() { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }

        // IMouseEventRegistrar
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;
        public Bounds Bounds => _collider.bounds;

        // IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerEnter(PointerEventData _) { Hovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerExit(PointerEventData _) { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
    }
}
