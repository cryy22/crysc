using System;
using Crysc.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Patterns.Registries
{
    public abstract class MouseEventRegistrar<T> : Registrar<T>, IMouseEventRegistrar<T>,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public Bounds Bounds => _genericSizeCalculator.Calculate().WorldBounds;
        private GenericSizeCalculator _genericSizeCalculator;

        protected override void Awake()
        {
            base.Awake();
            _genericSizeCalculator = new GenericSizeCalculator(this);
        }

        private void OnDisable() { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }

        // IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerEnter(PointerEventData _) { Hovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerExit(PointerEventData _) { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }

        // IMouseEventRegistrar
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;
    }
}
