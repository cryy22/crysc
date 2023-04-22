using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Presentation
{
    public abstract class CustomElementListPresenter<T, TPresenter> : MonoBehaviour
        where T : class
        where TPresenter : MonoBehaviour
    {
        [SerializeField] private TPresenter PresenterPrefab;
        [SerializeField] private Transform PresentersParent;
        [SerializeField] private Transform NoElementsIndicator;

        private readonly List<TPresenter> _items = new();

        public void SetElements(IEnumerable<T> elements)
        {
            bool hasElements = elements.Any();
            PresentersParent.gameObject.SetActive(hasElements);
            if (NoElementsIndicator) NoElementsIndicator.gameObject.SetActive(hasElements == false);
            if (hasElements == false) return;

            T[] elementsAry = elements.ToArray();
            int iterations = Mathf.Max(a: elementsAry.Length, b: _items.Count);

            for (var i = 0; i < iterations; i++)
            {
                if (i >= _items.Count)
                {
                    TPresenter item = Instantiate(original: PresenterPrefab, parent: PresentersParent);
                    _items.Add(item);
                }

                if (i >= elementsAry.Length)
                {
                    _items[i].gameObject.SetActive(false);
                    continue;
                }

                _items[i].gameObject.SetActive(true);
                SetElement(presenter: _items[i], element: elementsAry[i], index: i);
            }
        }

        protected abstract void SetElement(TPresenter presenter, T element, int index);
    }
}
