using System.Collections;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI.Presenters
{
    public class OffscreenPresenter : MonoBehaviour, IPresenter
    {
        [SerializeField] private Transform OffscreenPoint;
        [SerializeField] private Transform OnscreenPoint;
        [SerializeField] private Transform Container;

        [SerializeField] private float MoveTime = .25f;

        public PresentationState PresentationState { get; private set; }

        private CryRoutine _moveRoutine;
        private Vector3 OffscreenPosition => OffscreenPoint ? OffscreenPoint.localPosition : transform.localPosition;
        private Vector3 OnscreenPosition => OnscreenPoint ? OnscreenPoint.localPosition : transform.localPosition;

        private void Awake()
        {
            Container.localPosition = OffscreenPosition;
            PresentationState = PresentationState.Dismissed;
        }

        public void Present()
        {
            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: RunPresent(), behaviour: this);
        }

        public IEnumerator PresentAndWait()
        {
            Present();
            yield return new WaitUntil(() => _moveRoutine is { IsComplete: true });
        }

        public void Dismiss()
        {
            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: RunDismiss(), behaviour: this);
        }

        public IEnumerator DismissAndWait()
        {
            Dismiss();
            yield return new WaitUntil(() => _moveRoutine is { IsComplete: true });
        }

        private IEnumerator RunPresent()
        {
            if (PresentationState == PresentationState.Presented) yield break;
            if (PresentationState == PresentationState.Presenting) yield break;

            PresentationState = PresentationState.Presenting;
            yield return Mover.MoveToSmoothly(transform: Container, end: OnscreenPosition, duration: MoveTime);
            PresentationState = PresentationState.Presented;
        }

        private IEnumerator RunDismiss()
        {
            if (PresentationState == PresentationState.Dismissed) yield break;
            if (PresentationState == PresentationState.Dismissing) yield break;

            PresentationState = PresentationState.Dismissing;
            yield return Mover.MoveToSmoothly(transform: Container, end: OffscreenPosition, duration: MoveTime);
            PresentationState = PresentationState.Dismissed;
        }
    }
}
