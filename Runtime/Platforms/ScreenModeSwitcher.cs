using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Crysc.Platforms
{
    public class ScreenModeSwitcher : MonoBehaviour
    {
        private const string _exclusiveFullscreen = "Fullscreen";
        private const string _windowedFullscreen = "Borderless (Fullscreen)";
        private const string _maximizedWindow = "Maximized Window";
        private const string _windowed = "Windowed";

        [SerializeField] private TMP_Dropdown Dropdown;

        private void Awake()
        {
            List<TMP_Dropdown.OptionData> options = GetOptions();
            if (options.Count <= 1)
            {
                gameObject.SetActive(false);
                return;
            }

            Dropdown.options = options;

            Dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private List<TMP_Dropdown.OptionData> GetOptions()
        {
            var options = new List<TMP_Dropdown.OptionData>();

            if (Application.platform == RuntimePlatform.WindowsPlayer)
                options.Add(new TMP_Dropdown.OptionData(_exclusiveFullscreen));

            options.Add(new TMP_Dropdown.OptionData(_windowedFullscreen));

            if (Application.platform == RuntimePlatform.OSXPlayer)
                options.Add(new TMP_Dropdown.OptionData(_maximizedWindow));

            if (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.LinuxPlayer)
                options.Add(new TMP_Dropdown.OptionData(_windowed));

            return options;
        }

        private void OnValueChanged(int index)
        {
            string selected = Dropdown.options[index].text;

            Screen.fullScreenMode = selected switch
            {
                _exclusiveFullscreen => FullScreenMode.ExclusiveFullScreen,
                _windowedFullscreen  => FullScreenMode.FullScreenWindow,
                _maximizedWindow     => FullScreenMode.MaximizedWindow,
                _windowed            => FullScreenMode.Windowed,
                _                    => Screen.fullScreenMode,
            };
        }
    }
}
