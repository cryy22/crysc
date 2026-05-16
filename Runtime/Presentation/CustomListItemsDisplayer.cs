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

        protected readonly List<TItem> _items = new();
        protected int _count;

        protected void Awake()
        {
            if (_items.Capacity < InitialCapacity)
                _items.Capacity = InitialCapacity;
            
            if (gameObject.name == "AbilityIconItemsDisplayer")
                Debug.Log("howdy");

            for (int i = ItemsParent.childCount - 1; i >= 0; i--)
            {
                Transform child = ItemsParent.GetChild(i);
                child.gameObject.SetActive(false);

                var item = child.GetComponent<TItem>();
                if (item && (_items.Count < InitialCapacity))
                    _items.Insert(0, item);
                else
                    Destroy(child.gameObject);
            }

            for (int i = _items.Count; i < InitialCapacity; i++)
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
            EnsureCapacity(_count);

            ItemsParent.gameObject.SetActive(_count > 0);
            if (NoElementsIndicator)
                NoElementsIndicator.gameObject.SetActive(_count == 0);
            if (_count == 0)
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

        protected void EnsureCapacity(int count)
        {
            _items.Capacity = Mathf.Max(a: count, b: _items.Capacity);
            while (_items.Count < count)
            {
                TItem presenter = Instantiate(original: ItemPrefab, parent: ItemsParent);
                presenter.gameObject.SetActive(false);
                _items.Add(presenter);
            }
        }
    }
}
