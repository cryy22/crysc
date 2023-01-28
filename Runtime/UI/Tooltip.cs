using System;
using System.Collections;
using Crysc.Common;
using Crysc.Patterns.Registries;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Crysc.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class Tooltip<T> : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
        where T : Object
    {
        [SerializeField] private Registry<T> Registry;
        [SerializeField] protected GameObject Container;

        [SerializeField] private bool PersistsOnTooltipHover;
        [SerializeField] private bool MoveToTargetPosition;

        [SerializeField]
        [Range(min: 0, max: 1)]
        private float XFromCenter = 0.5f;

        [SerializeField]
        [Range(min: 0, max: 1)]
        private float YFromCenter = 0.5f;

        private Camera _camera;
        private BoundsCalculator _boundsCalculator;
        private T _target;

        private void Awake()
        {
            _camera = Camera.main;
            _boundsCalculator = new BoundsCalculator(Container.transform);

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

        public void OnPointerEnter(PointerEventData _)
        {
            if (PersistsOnTooltipHover) Container.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _) { HideTooltip(); }

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

        private void MoveTooltip(IMouseEventRegistrar<T> registrar) { StartCoroutine(RunMoveTooltip(registrar)); }

        private IEnumerator RunMoveTooltip(IMouseEventRegistrar<T> registrar)
        {
            yield return new WaitForEndOfFrame();
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
            float xInset = registrarBounds.extents.x * (1 - XFromCenter);
            float yInset = registrarBounds.extents.y * (1 - YFromCenter);

            float xValue = IsRegistrarOnLeft(registrar)
                ? registrarBounds.max.x - xInset
                : (registrarBounds.min.x - tooltipBounds.size.x) + xInset;

            float yValue = IsRegistrarOnBottom(registrar)
                ? registrarBounds.max.y - yInset
                : (registrarBounds.min.y - tooltipBounds.size.y) + yInset;

            var worldPoint = new Vector2(
                x: xValue,
                y: yValue
            );

            return _camera.WorldToScreenPoint(worldPoint);
        }

        private bool IsRegistrarOnLeft(IMouseEventRegistrar<T> registrar)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(registrar.Bounds.center);
            return screenPoint.x <= Screen.width / 2f;
        }

        private bool IsRegistrarOnBottom(IMouseEventRegistrar<T> registrar)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(registrar.Bounds.center);
            return screenPoint.y <= Screen.height / 2f;
        }

        private void UnhoveredEventHandler(object sender, EventArgs _) { HideTooltip(); }
    }
}
