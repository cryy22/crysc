using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI
{
    public class HyperlinkOpener : MonoBehaviour
    {
        [SerializeField] private Button Button;
        [SerializeField] private string Uri;

        private void Start() { Button.onClick.AddListener(OpenUri); }

        private void OpenUri() { Application.OpenURL(Uri); }
    }
}
