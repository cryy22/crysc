using System;
using System.Collections;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.UI.Presenters
{
    public class MultipointPresenter : MonoBehaviour, IPresenter
    {
        public event EventHandler Dismissed;
        public event EventHandler Presented;

        [FormerlySerializedAs("OffscreenPoint")] [SerializeField] private Transform DismissedPoint;
        [FormerlySerializedAs("OnscreenPoint")] [SerializeField] private Transform PresentedPoint;
        [SerializeField] private Transform Container;
        [SerializeField] private float MoveTime = 0.25f;

        public PresentationState PresentationState { get; private set; }

        private CryRoutine _moveRoutine;
        private Vector3 DismissedPosition => DismissedPoint ? DismissedPoint.localPosition : transform.localPosition;
        private Vector3 PresentedPosition => PresentedPoint ? PresentedPoint.localPosition : transform.localPosition;

        private void Awake()
        {
            Container.localPosition = DismissedPosition;
            PresentationState = PresentationState.Dismissed;
        }

        public void Present()
        {
            if (PresentationState == PresentationState.Presented) return;

            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: RunPresent(), behaviour: this);
        }

        public void Dismiss()
        {
            if (PresentationState == PresentationState.Dismissed) return;

            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: RunDismiss(), behaviour: this);
        }

        private IEnumerator RunPresent()
        {
            PresentationState = PresentationState.Presenting;
            yield return Mover.MoveToSmoothly(transform: Container, end: PresentedPosition, duration: MoveTime);

            PresentationState = PresentationState.Presented;
            Presented?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private IEnumerator RunDismiss()
        {
            PresentationState = PresentationState.Dismissing;
            yield return Mover.MoveToSmoothly(transform: Container, end: DismissedPosition, duration: MoveTime);

            PresentationState = PresentationState.Dismissed;
            Dismissed?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
