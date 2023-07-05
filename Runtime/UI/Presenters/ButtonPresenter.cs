using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Presenters
{
    public class ButtonPresenter : MonoBehaviour, IPresenter
    {
        public event EventHandler Dismissed
        {
            add => Presenter.Dismissed += value;
            remove => Presenter.Dismissed -= value;
        }

        public event EventHandler Presented
        {
            add => Presenter.Presented += value;
            remove => Presenter.Presented -= value;
        }

        [SerializeField] private Button ButtonInput;
        [SerializeField] private TMP_Text TextInput;
        [SerializeField] private Component PresenterInput;

        public Button Button => ButtonInput;
        public TMP_Text Text => TextInput;
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
