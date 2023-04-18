using System;
using System.Collections;
using System.Linq;
using Crysc.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    public abstract class Tooltip<T> : MonoBehaviour where T : TooltipContent
    {
        [SerializeField] private TooltipHoverPublisher Publisher;

        [SerializeField] protected RectTransform Container;
        [SerializeField] private bool MoveToTargetPosition;

        [SerializeField] [Range(min: 0, max: 2)] private float XFromCenter = 0.5f;
        [SerializeField] [Range(min: 0, max: 2)] private float YFromCenter = 0.5f;

        protected ITooltipTargetProvider Target;
        protected T Content;

        private readonly Vector3 _offScreen = new(x: -1000, y: -1000, z: 0);

        private Camera _camera;
        private GenericSizeCalculator _genericSizeCalculator;

        protected virtual void Awake()
        {
            _camera = Camera.main;
            _genericSizeCalculator = new GenericSizeCalculator(Container);

            DismissTooltip();
        }

        protected virtual void OnEnable() { Publisher.Hovered += HoveredEventHandler; }
        protected virtual void OnDisable() { Publisher.Hovered -= HoveredEventHandler; }

        protected virtual void DismissTooltip()
        {
            Container.gameObject.SetActive(false);
            Target.Unhovered -= UnhoveredEventHandler;
        }

        protected virtual void PresentTooltip(ITooltipTargetProvider target, T content)
        {
            Target = target;
            Content = content;

            Container.gameObject.SetActive(true);
            Target.Unhovered += UnhoveredEventHandler;
        }

        private static bool CanPresentData(TooltipContent content) { return content is T; }

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

        private void HoveredEventHandler(object sender, TooltipHoverEventArgs e)
        {
            if (e.TooltipContent.FirstOrDefault(CanPresentData) is not T content) return;

            PresentTooltip(target: e.TargetProvider, content: content);
            StartCoroutine(RunMoveTooltip(targetProvider: e.TargetProvider, targetBounds: e.Bounds));
        }

        private IEnumerator RunMoveTooltip(ITooltipTargetProvider targetProvider, Bounds targetBounds)
        {
            transform.position = _offScreen;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(Container);

            // required to ensure layout of variably-sized tooltips is complete.
            for (var i = 0; i < 3; i++) yield return null;

            Vector2 destination = MoveToTargetPosition
                ? GetTooltipScreenPoint(targetProvider: targetProvider, targetBounds: targetBounds)
                : transform.position;

            transform.position = new Vector3(
                x: destination.x,
                y: destination.y,
                z: transform.position.z
            );
        }

        private Vector2 GetTooltipScreenPoint(ITooltipTargetProvider targetProvider, Bounds targetBounds)
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

        private void UnhoveredEventHandler(object sender, EventArgs _) { DismissTooltip(); }
    }
}
