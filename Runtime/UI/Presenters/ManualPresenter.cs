using System;
using System.Collections;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI.Presenters
{
    public class ManualPresenter : MonoBehaviour, IPresenter
    {
        public event EventHandler Dismissed;
        public event EventHandler Presented;

        [SerializeField] private PresentationPoint DismissedPoint;
        [SerializeField] private PresentationPoint PresentedPoint;
        [SerializeField] private Transform ContainerInput;
        [SerializeField] private float MoveTime = 0.25f;

        public PresentationState PresentationState { get; private set; }

        private Transform Container => ContainerInput ?? transform;
        private Vector3 DismissedPosition => DismissedPoint.LocalPosition(Container.parent);
        private Vector3 PresentedPosition => PresentedPoint.LocalPosition(Container.parent);

        private CryRoutine _moveRoutine;

        private void Awake()
        {
            Container.localPosition = DismissedPosition;
            PresentationState = PresentationState.Dismissed;
        }

        public void Present()
        {
            if (PresentationState == PresentationState.Presented) return;

            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: Run(), behaviour: this);
            return;

            IEnumerator Run()
            {
                PresentationState = PresentationState.Presenting;
                yield return Mover.MoveToSmoothly(transform: Container, end: PresentedPosition, duration: MoveTime);

                PresentationState = PresentationState.Presented;
                Presented?.Invoke(sender: this, e: EventArgs.Empty);
            }
        }

        public void Dismiss()
        {
            if (PresentationState == PresentationState.Dismissed) return;

            _moveRoutine?.Stop();
            _moveRoutine = new CryRoutine(enumerator: Run(), behaviour: this);
            return;

            IEnumerator Run()
            {
                PresentationState = PresentationState.Dismissing;
                yield return Mover.MoveToSmoothly(transform: Container, end: DismissedPosition, duration: MoveTime);

                PresentationState = PresentationState.Dismissed;
                Dismissed?.Invoke(sender: this, e: EventArgs.Empty);
            }
        }
    }
}
