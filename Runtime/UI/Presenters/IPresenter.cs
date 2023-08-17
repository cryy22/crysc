using System;
using System.Collections;
using UnityEngine;

namespace Crysc.UI.Presenters
{
    public interface IPresenter
    {
        public event EventHandler Dismissed;
        public event EventHandler Presented;

        PresentationState PresentationState { get; }
        void Present();
        void Dismiss();
    }

    public static class PresenterExtensions
    {
        public static IEnumerator PresentAndWait(this IPresenter presenter)
        {
            presenter.Present();
            yield return new WaitUntil(() => presenter.PresentationState != PresentationState.Presenting);
        }

        public static IEnumerator DismissAndWait(this IPresenter presenter)
        {
            presenter.Dismiss();
            yield return new WaitUntil(() => presenter.PresentationState != PresentationState.Dismissing);
        }
    }
}
