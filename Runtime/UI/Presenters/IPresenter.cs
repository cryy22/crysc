using System;
using System.Collections;

namespace Crysc.UI.Presenters
{
    public interface IPresenter
    {
        public event EventHandler Dismissed;
        public event EventHandler Presented;

        PresentationState PresentationState { get; }
        void Present();
        IEnumerator PresentAndWait();
        void Dismiss();
        IEnumerator DismissAndWait();
    }
}
