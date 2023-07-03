using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Crysc.Presentation
{
    public class IconCountDisplay<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T IconPrefab;
        [SerializeField] private Transform Container;
        [SerializeField] private TMP_Text NumeralCounter;

        [SerializeField] private int NumeralCounterThreshold = 4;

        private readonly List<T> _icons = new();

        public void Set(int count)
        {
            if ((count >= NumeralCounterThreshold || count <= 0) && NumeralCounter)
            {
                DisplayIconCount(1);
                NumeralCounter.text = $"x {count.ToString()}";
                NumeralCounter.gameObject.SetActive(true);
                return;
            }

            DisplayIconCount(count);
            if (NumeralCounter) NumeralCounter.gameObject.SetActive(false);
        }

        private void DisplayIconCount(int count)
        {
            int difference = count - _icons.Count;

            if (difference > 0)
                for (var i = 0; i < difference; i++)
                    _icons.Add(Instantiate(original: IconPrefab, parent: Container));
            else if (difference < 0)
                for (var i = 0; i < -difference; i++)
                {
                    T icon = _icons.Last();
                    _icons.Remove(item: icon);
                    Destroy(icon.gameObject);
                }
        }
    }

    public class IconCountDisplay : IconCountDisplay<MonoBehaviour>
    { }
}
