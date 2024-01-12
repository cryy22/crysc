using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Crysc.Platforms
{
    public class ResolutionSwitcher : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown Dropdown;
        [SerializeField] private float MinAspectRatio = 16f / 10f;
        [SerializeField] private float MaxAspectRatio = 16f / 9f;

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

            string currentResolution = StringForResolution(width: Screen.width, height: Screen.height);
            int selected = options.FindIndex(o => o.text == currentResolution);
            if (selected < 0)
            {
                Array.Resize(array: ref _resolutions, newSize: _resolutions.Length + 1);
                _resolutions[^1] = Screen.currentResolution;

                selected = Dropdown.options.Count;
                Dropdown.options.Add(new TMP_Dropdown.OptionData(currentResolution));
            }

            Dropdown.SetValueWithoutNotify(selected);
            Dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private List<TMP_Dropdown.OptionData> GetOptions()
        {
            _resolutions = Screen.resolutions
                .Where(
                    r => r.width / (float) r.height >= MinAspectRatio && r.width / (float) r.height <= MaxAspectRatio
                )
                .OrderByDescending(r => r.width)
                .ThenByDescending(r => r.height)
                .ThenByDescending(r => r.refreshRateRatio.value)
                .GroupBy(r => r.width ^ r.height)
                .Select(g => g.First())
                .ToArray();

            return _resolutions
                .Select(r => StringForResolution(width: r.width, height: r.height))
                .Select(rName => new TMP_Dropdown.OptionData(rName))
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

        private static string StringForResolution(int width, int height)
        {
            return $"{width.ToString()}x{height.ToString()}";
        }
    }
}
