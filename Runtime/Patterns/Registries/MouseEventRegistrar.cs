using System;
using Crysc.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Patterns.Registries
{
    public abstract class MouseEventRegistrar<T> : Registrar<T>, IMouseEventRegistrar<T>,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private BoundsCalculator _boundsCalculator;

        protected override void Awake()
        {
            base.Awake();
            _boundsCalculator = new BoundsCalculator(this);
        }

        private void OnDisable() { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }

        // IMouseEventRegistrar
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;
        public Bounds Bounds => _boundsCalculator.Calculate();

        // IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerEnter(PointerEventData _) { Hovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerExit(PointerEventData _) { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
    }
}
