using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation
{
    public abstract class CustomListItemsDisplayer<T, TItem> : MonoBehaviour
        where TItem : MonoBehaviour
    {
        [FormerlySerializedAs("PresenterPrefab")] [SerializeField] private TItem ItemPrefab;
        [FormerlySerializedAs("PresentersParent")] [SerializeField] private Transform ItemsParent;
        [SerializeField] private Transform NoElementsIndicator;

        public IEnumerable<TItem> Items => _items;

        private readonly List<TItem> _items = new();

        public virtual void SetElements(IEnumerable<T> elements, bool ignoreNullElements = true)
        {
            bool hasElements = elements.Any();
            ItemsParent.gameObject.SetActive(hasElements);
            if (NoElementsIndicator) NoElementsIndicator.gameObject.SetActive(hasElements == false);
            if (hasElements == false) return;

            T[] elementsAry = elements.ToArray();
            int iterations = Mathf.Max(a: elementsAry.Length, b: _items.Count);

            for (var i = 0; i < iterations; i++)
            {
                TItem presenter = _items.ElementAtOrDefault(i);
                if (presenter == null)
                {
                    presenter = Instantiate(original: ItemPrefab, parent: ItemsParent);
                    _items.Add(presenter);
                }

                T element = elementsAry.ElementAtOrDefault(i);
                bool ignore = ignoreNullElements && element == null;
                presenter.gameObject.SetActive(ignore == false);
                if (ignore == false) SetElement(presenter: presenter, element: element, index: i);
            }
        }

        protected abstract void SetElement(TItem presenter, T element, int index);
    }
}
