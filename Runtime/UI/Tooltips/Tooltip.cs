using System;
using System.Collections;
using System.Linq;
using Crysc.Common;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    public abstract class Tooltip<T> : MonoBehaviour
    {
        [SerializeField] private TooltipHoverPublisher PublisherInput;
        [SerializeField] protected RectTransform Container;
        [SerializeField] private bool MoveToTargetPosition;
        [SerializeField] private bool SetInactiveWhenDismissed = true;

        [SerializeField] [Range(min: 0, max: 2)] private float XFromCenter = 0.5f;
        [SerializeField] [Range(min: 0, max: 2)] private float YFromCenter = 0.5f;

        private readonly Vector3 _offScreen = new(x: -10000, y: -10000, z: 0);

        private ITooltipTargetProvider _currentTarget;

        private Camera _camera;
        private GenericSizeCalculator _genericSizeCalculator;
        private CryRoutine _updatePositionRoutine;

        public bool IsActive { get; private set; } = true;

        private TooltipHoverPublisher Publisher => PublisherInput;

        private void Awake()
        {
            _camera = Camera.main;
            _genericSizeCalculator = new GenericSizeCalculator(Container);

            DismissTooltip();
        }

        private void OnEnable() { Publisher.Hovered += HoveredEventHandler; }
        private void OnDisable() { Publisher.Hovered -= HoveredEventHandler; }

        public virtual void SetActive(bool active)
        {
            IsActive = active;
            if (!IsActive) DismissTooltip();
        }

        protected virtual void PresentTooltip(T[] contents) { Container.gameObject.SetActive(true); }

        protected virtual bool ShouldPresentTooltip(T[] contents) { return IsActive; }

        protected virtual void DismissTooltip()
        {
            _updatePositionRoutine?.Stop();

            if (SetInactiveWhenDismissed) Container.gameObject.SetActive(false);

            if (_currentTarget == null) return;
            _currentTarget.Unhovered -= UnhoveredEventHandler;
            _currentTarget = null;
        }

        private void HoveredEventHandler(object sender, TooltipHoverEventArgs e)
        {
            T[] contents = e.TooltipContent.Where(c => c is T).Cast<T>().ToArray();
            if (contents.Length == 0) return;
            if (ShouldPresentTooltip(contents) == false) return;

            PresentTooltip(contents);
            UpdateCurrentTarget(e.TargetProvider);

            _updatePositionRoutine?.Stop();
            _updatePositionRoutine = new CryRoutine(
                enumerator: RunUpdateTooltipPosition(targetBounds: e.Bounds),
                behaviour: this
            );
        }

        private void UpdateCurrentTarget(ITooltipTargetProvider target)
        {
            if (_currentTarget != null) _currentTarget.Unhovered -= UnhoveredEventHandler;
            _currentTarget = target;
            _currentTarget.Unhovered += UnhoveredEventHandler;
        }

        private void UnhoveredEventHandler(object sender, EventArgs _) { DismissTooltip(); }

        private IEnumerator RunUpdateTooltipPosition(Bounds targetBounds)
        {
            transform.position = _offScreen;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(Container);

            // required to ensure layout of variably-sized tooltips is complete.
            for (var i = 0; i < 3; i++) yield return null;

            Vector2 destination = MoveToTargetPosition
                ? GetTooltipScreenPoint(targetBounds: targetBounds)
                : transform.position;

            transform.position = new Vector3(
                x: destination.x,
                y: destination.y,
                z: transform.position.z
            );
        }

        private Vector2 GetTooltipScreenPoint(Bounds targetBounds)
        {
            GenericSize tooltipSize = _genericSizeCalculator.Calculate();

            float xInset = targetBounds.extents.x * (1 - XFromCenter);
            float yInset = targetBounds.extents.y * (1 - YFromCenter);

            float xValue = IsTargetOnLeft(targetBounds)
                ? targetBounds.max.x - xInset
                : (targetBounds.min.x - tooltipSize.Bounds.size.x) + xInset;

            float yValue = IsTargetOnBottom(targetBounds)
                ? targetBounds.max.y - yInset
                : (targetBounds.min.y - tooltipSize.Bounds.size.y) + yInset;

            var worldPoint = new Vector2(
                x: xValue,
                y: yValue
            );

            return EnsureTooltipIsOnScreen(
                screenPoint: _camera.WorldToScreenPoint(worldPoint),
                tooltipSize: tooltipSize
            );
        }

        private bool IsTargetOnLeft(Bounds targetBounds)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(targetBounds.center);
            return screenPoint.x <= (Screen.width / 3f) * 2;
        }

        private bool IsTargetOnBottom(Bounds targetBounds)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(targetBounds.center);
            return screenPoint.y <= (Screen.height / 3f) * 2;
        }

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
    }
}
