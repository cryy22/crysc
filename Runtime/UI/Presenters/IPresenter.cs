using System.Collections;

namespace Crysc.UI.Presenters
{
    public interface IPresenter
    {
        PresentationState PresentationState { get; }
        void Present();
        IEnumerator PresentAndWait();
        void Dismiss();
        IEnumerator DismissAndWait();
    }
}
