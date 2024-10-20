using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Presenters
{
    public class TextLabelPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text TextInput;
        [SerializeField] private Image LabelInput;
        [SerializeField] private Component PresenterInput;

        public TMP_Text Text => TextInput;
        public Image Label => LabelInput;
        public PresentationState PresentationState => Presenter.PresentationState;

        public void Present() { Presenter.Present(); }
        public IEnumerator PresentAndWait() { return Presenter.PresentAndWait(); }
        public void Dismiss() { Presenter.Dismiss(); }
        public IEnumerator DismissAndWait() { return Presenter.DismissAndWait(); }

        #region Lazily-Initialized Properties

        private IPresenter _presenter;
        private IPresenter Presenter => _presenter != null ? _presenter : _presenter = CastPresenter();

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
