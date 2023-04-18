using System.Collections;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI
{
    public class OffscreenPresenter : MonoBehaviour
    {
        public enum PresentationState
        {
            Presenting,
            Presented,
            Dismissing,
            Dismissed,
        }

        [SerializeField] private Transform OffscreenPoint;
        [SerializeField] private Transform OnscreenPoint;
        [SerializeField] private Transform Container;

        [SerializeField] private float MoveTime = .25f;

        private CryRoutine _moveRoutine;

        private PresentationState State { get; set; } = PresentationState.Dismissed;
        private Vector3 OffscreenPosition => OffscreenPoint.localPosition;
        private Vector3 OnscreenPosition => OnscreenPoint.localPosition;

        private void Awake() { Container.localPosition = OffscreenPosition; }

        public void Present()
        {
            if (State == PresentationState.Presented) return;
            if (State == PresentationState.Presenting) return;

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
            if (State == PresentationState.Dismissed) return;
            if (State == PresentationState.Dismissing) return;

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
            State = PresentationState.Presenting;
            yield return Mover.MoveToSmoothly(transform: Container, end: OnscreenPosition, duration: MoveTime);
            State = PresentationState.Presented;
        }

        private IEnumerator RunDismiss()
        {
            State = PresentationState.Dismissing;
            yield return Mover.MoveToSmoothly(transform: Container, end: OffscreenPosition, duration: MoveTime);
            State = PresentationState.Dismissed;
        }
    }
}
