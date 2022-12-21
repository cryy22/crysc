using System;
using Crysc.Common;
using Crysc.Registries;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Crysc.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UITooltip<T> : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
        where T : Object
    {
        [SerializeField] private Registry<T> Registry;
        [SerializeField] protected GameObject Container;

        [SerializeField] private bool PersistsOnTooltipHover;
        [SerializeField] private bool MoveToTargetPosition;

        private Camera _camera;
        private BoundsCalculator _boundsCalculator;
        private T _target;

        private void Awake()
        {
            _camera = Camera.main;
            _boundsCalculator = new BoundsCalculator(this);

            HideTooltip();
        }

        protected virtual void OnEnable()
        {
            Registry.Hovered += HoveredEventHandler;
            Registry.Unhovered += UnhoveredEventHandler;
        }

        protected virtual void OnDisable()
        {
            Registry.Hovered -= HoveredEventHandler;
            Registry.Unhovered -= UnhoveredEventHandler;
        }

        protected virtual bool ShouldShowTooltip(T target) { return true; }

        protected virtual void ShowTooltip(T target)
        {
            if (ShouldShowTooltip(target) == false) return;

            UpdateTarget(target: target, previousTarget: _target);
            Container.SetActive(true);
        }

        protected virtual void UpdateTarget(T target, T previousTarget) { _target = target; }

        private void HideTooltip() { Container.SetActive(false); }

        private void HoveredEventHandler(object sender, RegistryEventArgs<T> e)
        {
            ShowTooltip(sender as T);

            if (MoveToTargetPosition && e.Registrar is IMouseEventRegistrar<T> meRegistrar) MoveTooltip(meRegistrar);
        }

        private void MoveTooltip(IMouseEventRegistrar<T> registrar)
        {
            Vector3 screenPoint = GetTooltipScreenPoint(registrar);
            transform.position = new Vector3(
                x: screenPoint.x,
                y: screenPoint.y,
                z: transform.position.z
            );
        }

        private Vector3 GetTooltipScreenPoint(IMouseEventRegistrar<T> registrar)
        {
            Bounds registrarBounds = registrar.Bounds;
            Bounds tooltipBounds = _boundsCalculator.Calculate();
            float xInset = registrarBounds.extents.x / 4;
            bool isRight = IsRegistrarOnRight(registrar);

            float xValue = isRight
                ? (registrarBounds.min.x - tooltipBounds.size.x) + xInset
                : registrarBounds.max.x - xInset;

            var worldPoint = new Vector2(
                x: xValue,
                y: registrarBounds.center.y + (registrarBounds.extents.y / 2)
            );

            return _camera.WorldToScreenPoint(worldPoint);
        }

        private bool IsRegistrarOnRight(IMouseEventRegistrar<T> registrar)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(registrar.Bounds.center);
            return screenPoint.x > (Screen.width / 3) * 2;
        }

        private void UnhoveredEventHandler(object sender, EventArgs _) { HideTooltip(); }

        public void OnPointerEnter(PointerEventData _)
        {
            if (PersistsOnTooltipHover) Container.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _) { HideTooltip(); }
    }
}
