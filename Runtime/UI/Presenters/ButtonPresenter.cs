using System;
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

        public void Present(bool interactable)
        {
            Button.interactable = interactable;
            Presenter.Present();
        }

        public void Dismiss(bool interactable)
        {
            Button.interactable = interactable;
            Presenter.Dismiss();
        }

        public void Present() { Present(true); }
        public void Dismiss() { Dismiss(false); }

        #region Lazily-Initialized Properties

        private IPresenter _presenter;
        private IPresenter Presenter => _presenter ??= CastPresenter();

        private IPresenter CastPresenter()
        {
            var presenter = PresenterInput as IPresenter;
            if (!PresenterInput || (presenter == null))
                throw new InvalidCastException($"PresenterInput {PresenterInput} is not an IPresenter");
            return presenter;
        }

        #endregion
    }
}
