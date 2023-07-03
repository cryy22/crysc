using System.Collections;
using Crysc.Common;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI.Presenters
{
    public class OffscreenPresenter : MonoBehaviour, IPresenter
    {
        public enum Origin
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
        }

        [SerializeField] private RectTransform Container;
        [SerializeField] private Origin ScreenOrigin;
        [SerializeField] private float OffscreenBuffer = 100f;
        [SerializeField] private float MoveTime = 0.25f;
        public PresentationState PresentationState { get; private set; }

        private CryRoutine _moveRoutine;
        private Vector3 _dismissedPosition;
        private Camera _camera;
        private Canvas _canvas;
        private RectTransform _canvasRect;

        private void Start()
        {
            _camera = Camera.main;

            _canvas = Container.GetComponentInParent<Canvas>();
            if (_canvas is null)
            {
                Debug.LogError("OffscreenPresenter's Container must be a child of a Canvas.");
                return;
            }

            _canvasRect = _canvas.GetComponent<RectTransform>();
            _dismissedPosition = CalculateOffscreenPosition();

            if (PresentationState is not PresentationState.None) return;
            Container.position = _dismissedPosition;
            Container.gameObject.SetActive(false);

            PresentationState = PresentationState.Dismissed;
        }

        public void Present()
        {
            if (PresentationState == PresentationState.Presented) return;

            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: RunPresent(), behaviour: this);
        }

        public IEnumerator PresentAndWait()
        {
            if (PresentationState == PresentationState.Presented) yield break;

            Present();
            yield return new WaitUntil(() => _moveRoutine is { IsComplete: true });
        }

        public void Dismiss()
        {
            if (PresentationState == PresentationState.Dismissed) return;

            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: RunDismiss(), behaviour: this);
        }

        public IEnumerator DismissAndWait()
        {
            if (PresentationState == PresentationState.Dismissed) yield break;

            Dismiss();
            yield return new WaitUntil(() => _moveRoutine is { IsComplete: true });
        }

        private IEnumerator RunPresent()
        {
            PresentationState = PresentationState.Presenting;

            Container.gameObject.SetActive(true);
            yield return Mover.MoveToSmoothly(
                transform: Container,
                end: Vector3.zero,
                duration: MoveTime,
                isLocal: true
            );

            PresentationState = PresentationState.Presented;
        }

        private IEnumerator RunDismiss()
        {
            PresentationState = PresentationState.Dismissing;

            yield return Mover.MoveToSmoothly(
                transform: Container,
                end: _dismissedPosition,
                duration: MoveTime,
                isLocal: false
            );
            Container.gameObject.SetActive(false);

            PresentationState = PresentationState.Dismissed;
        }

        private Vector3 CalculateOffscreenPosition()
        {
            Bounds bounds = GetCanvasScaledBounds();

            Vector3 difference = ScreenOrigin switch
            {
                Origin.Left   => (bounds.max.x + OffscreenBuffer) * Vector3.left,
                Origin.Right  => ((_canvasRect.rect.width - bounds.min.x) + OffscreenBuffer) * Vector3.right,
                Origin.Top    => ((_canvasRect.rect.height - bounds.min.y) + OffscreenBuffer) * Vector3.up,
                Origin.Bottom => (bounds.max.y + OffscreenBuffer) * Vector3.down,
                _             => Vector3.zero,
            };

            difference.Scale(
                ScreenOrigin switch
                {
                    Origin.Left or Origin.Right => Vector3.right,
                    Origin.Top or Origin.Bottom => Vector3.up,
                    _                           => Vector3.zero,
                }
            );

            Vector3 containerPosition = Container.position;
            if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
                containerPosition = _canvasRect.InverseTransformPoint(containerPosition);

            Vector3 offscreenPosition = containerPosition + difference;

            return _canvas.renderMode == RenderMode.ScreenSpaceCamera
                ? _canvasRect.TransformPoint(offscreenPosition)
                : offscreenPosition;
        }

        private Bounds GetCanvasScaledBounds()
        {
            var cameraToCanvasScale = new Vector3(
                x: _canvasRect.rect.width / _camera.pixelWidth,
                y: _canvasRect.rect.height / _camera.pixelHeight
            );

            Bounds cameraScaledBounds = new GenericSizeCalculator(Container).Calculate().ScreenBounds;
            Vector3 min = cameraScaledBounds.min;
            Vector3 max = cameraScaledBounds.max;

            min.Scale(cameraToCanvasScale);
            max.Scale(cameraToCanvasScale);

            Bounds canvasScaledBounds = new();
            canvasScaledBounds.SetMinMax(min: min, max: max);

            return canvasScaledBounds;
        }
    }
}
