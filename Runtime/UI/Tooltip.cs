using System;
using System.Collections;
using Crysc.Common;
using Crysc.Patterns.Registries;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Crysc.UI
{
    public abstract class Tooltip<T> : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
        where T : Object
    {
        [SerializeField] private Registry<T> Registry;
        [SerializeField] protected RectTransform Container;

        [SerializeField] private bool PersistsOnTooltipHover;
        [SerializeField] private bool MoveToTargetPosition;

        [SerializeField]
        [Range(min: 0, max: 1)]
        private float XFromCenter = 0.5f;

        [SerializeField]
        [Range(min: 0, max: 1)]
        private float YFromCenter = 0.5f;

        private static readonly Vector3 _offScreen = new(x: -1000, y: -1000, z: 0);

        private Camera _camera;
        private GenericSizeCalculator _genericSizeCalculator;
        private T _target;

        private void Awake()
        {
            _camera = Camera.main;
            _genericSizeCalculator = new GenericSizeCalculator(Container);

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
            if (PersistsOnTooltipHover) Container.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _) { HideTooltip(); }

        protected void HideTooltip() { Container.gameObject.SetActive(false); }

        protected virtual bool ShouldShowTooltip(T target) { return true; }

        protected virtual void ShowTooltip(T target)
        {
            if (ShouldShowTooltip(target) == false) return;

            UpdateTarget(target: target, previousTarget: _target);
            Container.gameObject.SetActive(true);
        }

        protected virtual void UpdateTarget(T target, T previousTarget) { _target = target; }

        private static Vector2 EnsureTooltipIsOnScreen(Vector3 screenPoint, GenericSize tooltipSize)
        {
            const float padding = 10;
            float xMax = Screen.width - tooltipSize.ScreenDimensions.x - padding;
            float yMax = Screen.height - tooltipSize.ScreenDimensions.y - padding;

            return new Vector2(
                x: xMax > padding ? Mathf.Clamp(value: screenPoint.x, min: padding, max: xMax) : padding,
                y: yMax > padding ? Mathf.Clamp(value: screenPoint.y, min: padding, max: yMax) : padding
            );
        }

        private void HoveredEventHandler(object sender, RegistryEventArgs<T> e)
        {
            ShowTooltip(sender as T);

            if (MoveToTargetPosition && e.Registrar is IMouseEventRegistrar<T> meRegistrar)
                StartCoroutine(RunMoveTooltip(meRegistrar));
        }

        private IEnumerator RunMoveTooltip(IMouseEventRegistrar<T> registrar)
        {
            transform.position = _offScreen;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(Container);

            // required to ensure layout of variably-sized tooltips is complete.
            yield return null;
            yield return null;

            Vector2 screenPoint = GetTooltipScreenPoint(registrar);
            transform.position = new Vector3(
                x: screenPoint.x,
                y: screenPoint.y,
                z: transform.position.z
            );
        }

        private Vector2 GetTooltipScreenPoint(IMouseEventRegistrar<T> registrar)
        {
            Bounds registrarBounds = registrar.Bounds;
            GenericSize tooltipSize = _genericSizeCalculator.Calculate();
            float xInset = registrarBounds.extents.x * (1 - XFromCenter);
            float yInset = registrarBounds.extents.y * (1 - YFromCenter);

            float xValue = IsRegistrarOnLeft(registrar)
                ? registrarBounds.max.x - xInset
                : (registrarBounds.min.x - tooltipSize.Bounds.size.x) + xInset;

            float yValue = IsRegistrarOnBottom(registrar)
                ? registrarBounds.max.y - yInset
                : (registrarBounds.min.y - tooltipSize.Bounds.size.y) + yInset;

            var worldPoint = new Vector2(
                x: xValue,
                y: yValue
            );

            return EnsureTooltipIsOnScreen(
                screenPoint: _camera.WorldToScreenPoint(worldPoint),
                tooltipSize: tooltipSize
            );
        }

        private bool IsRegistrarOnLeft(IMouseEventRegistrar<T> registrar)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(registrar.Bounds.center);
            return screenPoint.x <= (Screen.width / 3f) * 2;
        }

        private bool IsRegistrarOnBottom(IMouseEventRegistrar<T> registrar)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(registrar.Bounds.center);
            return screenPoint.y <= (Screen.height / 3f) * 2;
        }

        private void UnhoveredEventHandler(object sender, EventArgs _) { HideTooltip(); }
    }
}
