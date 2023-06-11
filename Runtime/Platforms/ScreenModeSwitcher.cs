using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Crysc.Platforms
{
    public class ScreenModeSwitcher : MonoBehaviour
    {
        private const string _exclusiveFullscreen = "Fullscreen";
        private const string _windowedFullscreen = "Fullscreen (Borderless)";
        private const string _maximizedWindow = "Maximized Window";
        private const string _windowed = "Windowed";

        private const string _custom = "Custom";

        [SerializeField] private TMP_Dropdown Dropdown;
        private FullScreenMode _customMode;

        private void Awake()
        {
            List<TMP_Dropdown.OptionData> options = GetOptions();
            if (options.Count <= 1 && Application.isEditor == false)
            {
                gameObject.SetActive(false);
                return;
            }

            Dropdown.options = options;

            int selected = options.FindIndex(o => o.text == GetStringForMode(Screen.fullScreenMode));
            if (selected < 0)
            {
                _customMode = Screen.fullScreenMode;
                selected = Dropdown.options.Count;
                Dropdown.options.Add(new TMP_Dropdown.OptionData(_custom));
            }

            Dropdown.SetValueWithoutNotify(selected);

            Dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private List<TMP_Dropdown.OptionData> GetOptions()
        {
            var options = new List<TMP_Dropdown.OptionData> { new(_windowedFullscreen) };

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                    options.Add(new TMP_Dropdown.OptionData(_exclusiveFullscreen));
                    options.Add(new TMP_Dropdown.OptionData(_windowed));
                    break;
                case RuntimePlatform.OSXPlayer:
                    options.Add(new TMP_Dropdown.OptionData(_maximizedWindow));
                    options.Add(new TMP_Dropdown.OptionData(_windowed));
                    break;
                case RuntimePlatform.LinuxPlayer:
                    options.Add(new TMP_Dropdown.OptionData(_windowed));
                    break;
            }

            return options;
        }

        private void OnValueChanged(int index)
        {
            Screen.fullScreenMode = GetModeForString(Dropdown.options[index].text);
        }

        private FullScreenMode GetModeForString(string mode)
        {
            return mode switch
            {
                _exclusiveFullscreen => FullScreenMode.ExclusiveFullScreen,
                _windowedFullscreen  => FullScreenMode.FullScreenWindow,
                _maximizedWindow     => FullScreenMode.MaximizedWindow,
                _windowed            => FullScreenMode.Windowed,
                _custom              => _customMode,
                _                    => Screen.fullScreenMode,
            };
        }

        private string GetStringForMode(FullScreenMode mode)
        {
            return mode switch
            {
                FullScreenMode.ExclusiveFullScreen => _exclusiveFullscreen,
                FullScreenMode.FullScreenWindow    => _windowedFullscreen,
                FullScreenMode.MaximizedWindow     => _maximizedWindow,
                FullScreenMode.Windowed            => _windowed,
                _                                  => _windowedFullscreen,
            };
        }
    }
}
