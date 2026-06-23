using System;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI
{
    public class HyperlinkOpener : MonoBehaviour
    {
        [SerializeField] private Button Button;
        [SerializeField] private string Uri;

        private EventButton _button;

        private void Awake()
        {
            _button = GetComponent<EventButton>();
        }

        private void OnEnable()
        {
            if (Button)
                Button.onClick.AddListener(OpenUri);

            if (_button != null)
                _button.Clicked += OpenUri;
        }

        private void OnDisable()
        {
            if (Button)
                Button.onClick.RemoveListener(OpenUri);

            if (_button != null)
                _button.Clicked -= OpenUri;
        }

        private void OpenUri()
        {
            Application.OpenURL(Uri);
        }

        private void OpenUri(object sender, EventArgs e)
        {
            Application.OpenURL(Uri);
        }
    }
}
