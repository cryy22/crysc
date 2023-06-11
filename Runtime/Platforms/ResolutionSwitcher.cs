using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Crysc.Platforms
{
    public class ResolutionSwitcher : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown Dropdown;

        private Resolution[] _resolutions;

        private void Awake()
        {
            List<TMP_Dropdown.OptionData> options = GetOptions();
            if (options.Count <= 1)
            {
                gameObject.SetActive(false);
                return;
            }

            Dropdown.options = options;
            // select the option that matches the current state
            Dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private List<TMP_Dropdown.OptionData> GetOptions()
        {
            _resolutions = Screen.resolutions.Reverse().ToArray();

            return _resolutions
                .Select(resolution => $"{resolution.width.ToString()}x{resolution.height.ToString()}")
                .Select(resolutionName => new TMP_Dropdown.OptionData(resolutionName))
                .ToList();
        }

        private void OnValueChanged(int index)
        {
            Resolution resolution = _resolutions[index];
            Screen.SetResolution(
                width: resolution.width,
                height: resolution.height,
                fullscreenMode: Screen.fullScreenMode
            );
        }
    }
}
