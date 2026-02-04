using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation
{
    public abstract class CustomListItemsDisplayer<T, TItem> : MonoBehaviour
        where TItem : MonoBehaviour
    {
        [SerializeField] private int InitialCapacity = 2;
        [FormerlySerializedAs("PresenterPrefab")] [SerializeField] private TItem ItemPrefab;
        [FormerlySerializedAs("PresentersParent")] [SerializeField] private Transform ItemsParent;
        [SerializeField] private Transform NoElementsIndicator;

        public IEnumerable<TItem> Items => _items.Take(_count);

        private readonly List<TItem> _items = new();
        private int _count;

        protected void Awake()
        {
            _items.Capacity = InitialCapacity;

            for (var i = 0; i < InitialCapacity; i++)
            {
                TItem presenter = Instantiate(original: ItemPrefab, parent: ItemsParent);
                presenter.gameObject.SetActive(false);
                _items.Add(presenter);
            }
        }

        public virtual void SetElements(IEnumerable<T> elements, bool ignoreNullElements = true)
        {
            T[] elementsAry = elements.ToArray();
            _count = elementsAry.Length;
            EnsureCapacity();

            ItemsParent.gameObject.SetActive(elementsAry.Length > 0);
            if (NoElementsIndicator)
                NoElementsIndicator.gameObject.SetActive(elementsAry.Length == 0);
            if (elementsAry.Length == 0)
                return;

            for (var i = 0; i < _items.Count; i++)
            {
                TItem presenter = _items[i];
                if (i < elementsAry.Length)
                {
                    bool ignore = ignoreNullElements && (elementsAry[i] == null);
                    presenter.gameObject.SetActive(!ignore);
                    if (!ignore)
                        SetElement(presenter: presenter, element: elementsAry[i], index: i);
                }
                else
                {
                    presenter.gameObject.SetActive(false);
                }
            }
        }

        protected abstract void SetElement(TItem presenter, T element, int index);

        private void EnsureCapacity()
        {
            _items.Capacity = Mathf.Max(a: _count, b: _items.Capacity);
            while (_items.Count < _count)
            {
                TItem presenter = Instantiate(original: ItemPrefab, parent: ItemsParent);
                presenter.gameObject.SetActive(false);
                _items.Add(presenter);
            }
        }
    }
}
