using System.Collections;
using Crysc.Common;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    public class TooltipPositioner : MonoBehaviour
    {
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

            Vector2 destination = MoveToTargetPosition
                ? GetTooltipScreenPoint(targetDimensions: targetDimensions)
                : transform.position;

            transform.position = new Vector3(
                x: destination.x,
                y: destination.y,
                z: transform.position.z
            );
        }

        private Vector2 GetTooltipScreenPoint(Dimensions targetDimensions)
        {
            Dimensions tooltipDimensions = SizeCalculator.Calculate();

            float xInset = targetDimensions.WorldBounds.extents.x * (1 - XFromCenter);
            float yInset = targetDimensions.WorldBounds.extents.y * (1 - YFromCenter);

            float xValue = IsTargetOnLeft(targetDimensions)
                ? targetDimensions.WorldBounds.max.x - xInset
                : (targetDimensions.WorldBounds.min.x - tooltipDimensions.WorldBounds.size.x) + xInset;

            float yValue = IsTargetOnBottom(targetDimensions)
                ? targetDimensions.WorldBounds.max.y - yInset
                : (targetDimensions.WorldBounds.min.y - tooltipDimensions.WorldBounds.size.y) + yInset;

            var worldPoint = new Vector2(
                x: xValue,
                y: yValue
            );

            return EnsureTooltipIsOnScreen(
                screenPoint: Camera.WorldToScreenPoint(worldPoint),
                tooltipScreenSize: tooltipDimensions.ScreenBounds.size
            );
        }

        private static bool IsTargetOnLeft(Dimensions targetDimensions)
        {
            return targetDimensions.ScreenBounds.center.x <= (Screen.width / 3f) * 2;
        }

        private static bool IsTargetOnBottom(Dimensions targetDimensions)
        {
            return targetDimensions.ScreenBounds.center.y <= (Screen.height / 3f) * 2;
        }

        private static Vector2 EnsureTooltipIsOnScreen(Vector3 screenPoint, Vector2 tooltipScreenSize)
        {
            const float padding = 10;

            float xMax = Screen.width - tooltipScreenSize.x - padding;
            float yMax = Screen.height - tooltipScreenSize.y - padding;

            return new Vector2(
                x: xMax > padding ? Mathf.Clamp(value: screenPoint.x, min: padding, max: xMax) : padding,
                y: yMax > padding ? Mathf.Clamp(value: screenPoint.y, min: padding, max: yMax) : padding
            );
        }
    }
}
