using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI
{
    using IElement = IArrangementElement;

    public class UIArrangement : MonoBehaviour
    {
        private const float _zOffset = 0.000001f;

        [SerializeField] private Vector2 ElementSpacing = Vector2.right;
        [SerializeField] private Vector2 ElementStagger = Vector2.zero;
        [SerializeField] private bool IsOrderInverted;

        [SerializeField] private Transform ElementsParent;

        private readonly Dictionary<IElement, Vector3> _elementsPositions = new();

        public void InvertOrder()
        {
            IsOrderInverted = !IsOrderInverted;
            UpdateElements(_elementsPositions.Keys);
        }

        public void UpdateElements(IEnumerable<IElement> elements)
        {
            UpdateElementsAndPositions(elements);
            foreach (IElement element in _elementsPositions.Keys)
                element.Transform.localPosition = _elementsPositions[element];
        }

        public IEnumerator AnimateUpdateElements(IEnumerable<IElement> elements)
        {
            bool hasChanged = UpdateElementsAndPositions(elements);
            if (!hasChanged) yield break;

            Coroutine[] coroutines = _elementsPositions.Keys
                .Select(
                    e => StartCoroutine(
                        Mover.MoveLocal(transform: e.Transform, end: _elementsPositions[e], duration: 0.25f)
                    )
                )
                .ToArray();

            yield return CoroutineWaiter.RunConcurrently(coroutines);
        }

        private bool UpdateElementsAndPositions(IEnumerable<IElement> elements)
        {
            elements = elements.ToList();
            var hasChanged = false;

            var positionedElements = 0;
            foreach (IElement element in elements)
            {
                if (UpdateElementAndPosition(element: element, index: positionedElements)) hasChanged = true;
                positionedElements++;
            }

            List<IElement> removedElements = _elementsPositions.Keys.Except(elements).ToList();
            if (removedElements.Count > 0) hasChanged = true;
            foreach (IElement element in removedElements) _elementsPositions.Remove(element);

            return hasChanged;
        }

        private bool UpdateElementAndPosition(IElement element, int index)
        {
            Vector2 localPosition2D = ElementSpacing * (index * (IsOrderInverted ? -1 : 1));
            localPosition2D += ElementStagger * (index % 2);

            var localPosition = new Vector3(
                x: localPosition2D.x,
                y: localPosition2D.y,
                z: _zOffset * index
            );
            if (_elementsPositions.ContainsKey(element) && _elementsPositions[element] == localPosition) return false;

            element.Transform.SetParent(ElementsParent);
            element.Transform.gameObject.SetActive(true);
            element.Transform.localScale = Vector3.one;

            _elementsPositions[element] = localPosition;
            return true;
        }
    }
}
