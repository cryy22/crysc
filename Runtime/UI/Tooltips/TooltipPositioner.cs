using System.Collections;
using Crysc.Common;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    public class TooltipPositioner : MonoBehaviour
    {
        [SerializeField] protected RectTransform Container;
        [SerializeField] private bool MoveToTargetPosition;

        [SerializeField] [Range(min: 0, max: 2)] private float XFromCenter = 0.5f;
        [SerializeField] [Range(min: 0, max: 2)] private float YFromCenter = 0.5f;

        private readonly Vector3 _offScreen = new(x: -10000, y: -10000, z: 0);

        private Camera _camera;
        private Camera Camera => _camera ? _camera : _camera = Camera.main;
        private GenericSizeCalculator _sizeCalculator;
        private GenericSizeCalculator SizeCalculator => _sizeCalculator ??= new GenericSizeCalculator(Container);

        private ITooltipTargetProvider _currentTarget;
        private CryRoutine _updatePositionRoutine;

        public void HaltPositioning() { _updatePositionRoutine?.Stop(); }

        public void UpdateTooltipPosition(Bounds targetBounds)
        {
            _updatePositionRoutine?.Stop();
            _updatePositionRoutine = new CryRoutine(
                enumerator: RunUpdateTooltipPosition(targetBounds: targetBounds),
                behaviour: this
            );
        }

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
            GenericSize tooltipSize = SizeCalculator.Calculate();

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
                screenPoint: Camera.WorldToScreenPoint(worldPoint),
                tooltipSize: tooltipSize
            );
        }

        private bool IsTargetOnLeft(Bounds targetBounds)
        {
            Vector3 screenPoint = Camera.WorldToScreenPoint(targetBounds.center);
            return screenPoint.x <= (Screen.width / 3f) * 2;
        }

        private bool IsTargetOnBottom(Bounds targetBounds)
        {
            Vector3 screenPoint = Camera.WorldToScreenPoint(targetBounds.center);
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
