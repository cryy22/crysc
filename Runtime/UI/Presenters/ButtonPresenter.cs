using System;
using System.Collections;
using Crysc.UI.Presenters;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordination.Camps
{
    public class ButtonPresenter : MonoBehaviour, IPresenter
    {
        [SerializeField] private Button ButtonInput;
        [SerializeField] private Component PresenterInput;

        public Button Button => ButtonInput;
        public PresentationState PresentationState => Presenter.PresentationState;

        private void Awake() { Button.interactable = false; }

        public void Present()
        {
            Button.interactable = true;
            Presenter.Present();
        }

        public IEnumerator PresentAndWait()
        {
            Button.interactable = true;
            return Presenter.PresentAndWait();
        }

        public void Dismiss()
        {
            Button.interactable = false;
            Presenter.Dismiss();
        }

        public IEnumerator DismissAndWait()
        {
            Button.interactable = false;
            return Presenter.DismissAndWait();
        }

        #region Lazily-Initialized Properties

        private IPresenter _presenter;
        private IPresenter Presenter => _presenter != null ? _presenter : _presenter = CastPresenter();

        private IPresenter CastPresenter()
        {
            if (PresenterInput is not IPresenter presenter)
                throw new InvalidCastException($"PresenterInput {PresenterInput} is not an IPresenter");
            return presenter;
        }

        #endregion
    }
}
