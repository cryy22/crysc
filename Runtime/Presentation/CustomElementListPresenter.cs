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
                TPresenter presenter = _items.ElementAtOrDefault(i);
                if (presenter == null)
                {
                    presenter = Instantiate(original: PresenterPrefab, parent: PresentersParent);
                    _items.Add(presenter);
                }

                T element = elementsAry.ElementAtOrDefault(i);
                presenter.gameObject.SetActive(element != null);
                if (element != null) SetElement(presenter: presenter, element: element, index: i);
            }
        }

        protected abstract void SetElement(TPresenter presenter, T element, int index);
    }
}
