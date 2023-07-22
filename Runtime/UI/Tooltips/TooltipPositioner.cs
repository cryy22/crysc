using System.Collections;
using Crysc.Common;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    public class TooltipPositioner : MonoBehaviour
    {
        private const float _padding = 20;
        private static readonly Vector3 _offScreen = new(x: -10000, y: -10000, z: 0);

        [SerializeField] protected RectTransform Container;
        [SerializeField] private bool MoveToTargetPosition;

        [SerializeField] [Range(min: 0, max: 2)] private float XFromCenter = 0.5f;
        [SerializeField] [Range(min: 0, max: 2)] private float YFromCenter = 0.5f;

        private Camera _camera;
        private Camera Camera => _camera ? _camera : _camera = Camera.main;
        private GenericSizeCalculator _sizeCalculator;
        private GenericSizeCalculator SizeCalculator => _sizeCalculator ??= new GenericSizeCalculator(Container);

        private ITooltipTargetProvider _currentTarget;
        private CryRoutine _updatePositionRoutine;

        public void UpdateTooltipPosition(Dimensions targetDimensions)
        {
            _updatePositionRoutine?.Stop();
            _updatePositionRoutine = new CryRoutine(
                enumerator: RunUpdateTooltipPosition(targetDimensions),
                behaviour: this
            );
        }

        public void HandleDismissal()
        {
            _updatePositionRoutine?.Stop();
            transform.position = _offScreen;
        }

        public bool IsMouseOverTooltip() { return SizeCalculator.Calculate().IsScreenPointWithin(Input.mousePosition); }

        private IEnumerator RunUpdateTooltipPosition(Dimensions targetDimensions)
        {
            transform.position = _offScreen;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(Container);

            // required to ensure layout of variably-sized tooltips is complete.
            for (var i = 0; i < 3; i++) yield return null;

            Dimensions tooltipDimensions = SizeCalculator.Calculate();
            transform.localScale = GetScreenFittingScale(
                tooltipScreenSize: tooltipDimensions.ScreenBounds.size,
                currentScale: transform.localScale
            );
            tooltipDimensions = SizeCalculator.Calculate();

            Vector2 destination = MoveToTargetPosition
                ? GetTooltipScreenPoint(targetDimensions: targetDimensions, tooltipDimensions: tooltipDimensions)
                : transform.position;

            transform.position = new Vector3(
                x: destination.x,
                y: destination.y,
                z: transform.position.z
            );
        }

        private Vector2 GetTooltipScreenPoint(Dimensions targetDimensions, Dimensions tooltipDimensions)
        {
            float xInset = targetDimensions.WorldBounds.extents.x * (1 - XFromCenter);
            float yInset = targetDimensions.WorldBounds.extents.y * (1 - YFromCenter);

            float xValue = ShouldTooltipBeOnRight(targetDimensions: targetDimensions)
                ? targetDimensions.WorldBounds.max.x - xInset
                : (targetDimensions.WorldBounds.min.x - tooltipDimensions.WorldBounds.size.x) + xInset;

            float yValue = ShouldTooltipBeAbove(targetDimensions: targetDimensions)
                ? targetDimensions.WorldBounds.max.y - yInset
                : (targetDimensions.WorldBounds.min.y - tooltipDimensions.WorldBounds.size.y) + yInset;

            var worldPoint = new Vector2(
                x: xValue,
                y: yValue
            );

            return Camera.ScreenToWorldPoint(
                EnsureTooltipIsOnScreen(
                    screenPoint: Camera.WorldToScreenPoint(worldPoint),
                    tooltipScreenSize: tooltipDimensions.ScreenBounds.size
                )
            );
        }

        private static bool ShouldTooltipBeOnRight(Dimensions targetDimensions)
        {
            return targetDimensions.ScreenBounds.center.x <= (Screen.width / 5f) * 3;
        }

        private static bool ShouldTooltipBeAbove(Dimensions targetDimensions)
        {
            return targetDimensions.ScreenBounds.center.y <= (Screen.height / 5f) * 3;
        }

        private static Vector2 EnsureTooltipIsOnScreen(Vector3 screenPoint, Vector2 tooltipScreenSize)
        {
            float xMax = Screen.width - tooltipScreenSize.x - _padding;
            float yMax = Screen.height - tooltipScreenSize.y - _padding;

            return new Vector2(
                x: xMax > _padding ? Mathf.Clamp(value: screenPoint.x, min: _padding, max: xMax) : _padding,
                y: yMax > _padding ? Mathf.Clamp(value: screenPoint.y, min: _padding, max: yMax) : _padding
            );
        }

        private static Vector3 GetScreenFittingScale(Vector2 tooltipScreenSize, Vector3 currentScale)
        {
            float maxFittingScaleModifier = Mathf.Min(
                a: (Screen.width - (2 * _padding)) / tooltipScreenSize.x,
                b: (Screen.height - (2 * _padding)) / tooltipScreenSize.y
            );
            float minDimension = Mathf.Min(a: currentScale.x, b: currentScale.y);
            float scaleModifier = Mathf.Min(a: 1 / minDimension, b: maxFittingScaleModifier);
            return currentScale * scaleModifier;
        }
    }
}
