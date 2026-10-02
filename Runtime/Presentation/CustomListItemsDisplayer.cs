#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Crysc.Presentation
{
    public abstract class CustomListItemsDisplayer<T, TItem> : MonoBehaviour
        where TItem : MonoBehaviour
    {
        [SerializeField] private int InitialCapacity = 2;
        [FormerlySerializedAs("PresenterPrefab")] [SerializeField] private TItem ItemPrefab;
        [FormerlySerializedAs("PresentersParent")] [SerializeField] private Transform ItemsParent;
        [SerializeField] private Transform NoElementsIndicator;

        protected readonly List<TItem> Items = new();
        protected int ActiveCount;

        protected void Awake()
        {
            if (Items.Capacity < InitialCapacity)
                Items.Capacity = InitialCapacity;

            for (int i = ItemsParent.childCount - 1; i >= 0; i--)
            {
                Transform child = ItemsParent.GetChild(i);
                child.gameObject.SetActive(false);

                var item = child.GetComponent<TItem>();
                if (item && (Items.Count < InitialCapacity))
                    Items.Insert(index: 0, item: item);
                else
                    Destroy(child.gameObject);
            }

            for (int i = Items.Count; i < InitialCapacity; i++)
            {
                TItem presenter = Instantiate(original: ItemPrefab, parent: ItemsParent);
                presenter.gameObject.SetActive(false);
                Items.Add(presenter);
            }
        }

        public virtual void SetElements(IEnumerable<T> elements, bool ignoreNullElements = true)
        {
            T[] elementsAry = elements.ToArray();
            ActiveCount = elementsAry.Length;
            EnsureCapacity(ActiveCount);

            ItemsParent.gameObject.SetActive(ActiveCount > 0);
            if (NoElementsIndicator)
                NoElementsIndicator.gameObject.SetActive(ActiveCount == 0);
            if (ActiveCount == 0)
                return;

            for (var i = 0; i < Items.Count; i++)
            {
                TItem presenter = Items[i];
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
            Items.Capacity = Mathf.Max(a: count, b: Items.Capacity);
            while (Items.Count < count)
            {
                TItem presenter = Instantiate(original: ItemPrefab, parent: ItemsParent);
                presenter.gameObject.SetActive(false);
                Items.Add(presenter);
            }
        }
    }
}
