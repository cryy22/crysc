using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI
{
    public class UIArrangement : MonoBehaviour
    {
        private const float _zOffset = 0.000001f;

        [SerializeField] private Vector2 ElementSpacing = Vector2.right;
        [SerializeField] private Vector2 ElementStagger = Vector2.zero;
        [SerializeField] private Transform ElementsParent;

        private readonly Dictionary<Transform, Vector3> _elementsPositions = new();

        public void UpdateArrangement(IEnumerable<Transform> elements)
        {
            UpdateElementsAndPositions(elements);
            foreach (Transform element in _elementsPositions.Keys)
                element.localPosition = _elementsPositions[element];
        }

        public void AnimateUpdateArrangement(IEnumerable<Transform> elements)
        {
            bool hasChanged = UpdateElementsAndPositions(elements);
            if (!hasChanged) return;

            Coroutine[] coroutines = _elementsPositions.Keys
                .Select(
                    e => StartCoroutine(Mover.MoveLocal(transform: e, end: _elementsPositions[e], duration: 0.25f))
                )
                .ToArray();

            StartCoroutine(CoroutineWaiter.RunConcurrently(coroutines));
        }

        private bool UpdateElementsAndPositions(IEnumerable<Transform> elements)
        {
            elements = elements.ToList();
            var hasChanged = false;

            var positionedElements = 0;
            foreach (Transform element in elements)
            {
                if (UpdateElementAndPosition(element: element, index: positionedElements)) hasChanged = true;
                positionedElements++;
            }

            List<Transform> removedElements = _elementsPositions.Keys.Except(elements).ToList();
            if (removedElements.Count > 0) hasChanged = true;
            foreach (Transform element in removedElements) _elementsPositions.Remove(element);

            return hasChanged;
        }

        private bool UpdateElementAndPosition(Transform element, int index)
        {
            Vector2 localPosition = index * ElementSpacing;
            localPosition += ElementStagger * (index % 2);

            Vector3 position = ElementsParent.TransformPoint(
                new Vector3(
                    x: localPosition.x,
                    y: localPosition.y,
                    z: _zOffset * index
                )
            );

            if (_elementsPositions.ContainsKey(element) && _elementsPositions[element] == position) return false;

            element.SetParent(ElementsParent);
            element.gameObject.SetActive(true);
            element.localScale = Vector3.one;

            _elementsPositions[element] = position;
            return true;
        }
    }
}
