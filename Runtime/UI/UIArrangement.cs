using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.UI
{
    using IElement = IArrangementElement;

    public class UIArrangement : MonoBehaviour, IElement
    {
        private const float _zOffset = 0.000001f;

        [FormerlySerializedAs("Direction")]
        [SerializeField]
        private Vector2 PreferredDirection = Vector2.right;

        [SerializeField] private Vector2 ElementStagger = Vector2.zero;
        [SerializeField] private Vector2 MaxSize = Vector2.positiveInfinity;
        [SerializeField] private bool IsOrderInverted;

        [SerializeField] private Transform ElementsParent;

        private readonly Dictionary<IElement, Vector3> _elementsPositions = new();
        private Vector2 _direction;

        // 1 - ARRANGEMENT SHOULD ACCEPT MAX SIZE AND STACK ELEMENTS AS NEEDED
        // 2 - ARRANGEMENT SHOULD BE ABLE TO ANIMATE SIZE CHANGES, ESPECIALLY MAX SIZE
        // 3 - NON-FRONT SQUAD ELEMENTS SHOULD BE SQUEEZED INTO THE BACK

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
            UpdateMaxSizeAwareDirection(elements);
            var hasChanged = false;

            var index = 0;
            IElement previous = null;
            foreach (IElement current in elements)
            {
                if (UpdateElementPosition(current: current, previous: previous, index: index)) hasChanged = true;

                index++;
                previous = current;
            }

            List<IElement> removedElements = _elementsPositions.Keys.Except(elements).ToList();
            if (removedElements.Count > 0) hasChanged = true;
            foreach (IElement element in removedElements) _elementsPositions.Remove(element);

            return hasChanged;
        }

        private bool UpdateElementPosition(IElement current, IElement previous, int index)
        {
            Vector3 localPosition = CalculateElementPosition(current: current, previous: previous, index: index);
            if (_elementsPositions.ContainsKey(current) && _elementsPositions[current] == localPosition) return false;

            current.Transform.SetParent(ElementsParent);
            current.Transform.gameObject.SetActive(true);
            current.Transform.localScale = Vector3.one;

            _elementsPositions[current] = localPosition;
            return true;
        }

        private Vector3 CalculateElementPosition(IElement current, IElement previous, int index)
        {
            if (index == 0) return Vector3.zero;

            Vector2 localPosition2D = _elementsPositions[previous];

            Vector2 distance = (previous.Bounds.extents + current.Bounds.extents) * _direction;
            localPosition2D += distance * (IsOrderInverted ? -1 : 1);
            localPosition2D += ElementStagger * (index % 2 == 0 ? 1 : -1);

            return new Vector3(
                x: localPosition2D.x,
                y: localPosition2D.y,
                z: _zOffset * index
            );
        }

        private void UpdateMaxSizeAwareDirection(IEnumerable<IElement> elements)
        {
            Vector2 totalSize = elements.Aggregate(
                seed: Vector2.zero,
                (acc, kvp) => acc + (Vector2) kvp.Bounds.size
            );
            Vector2 maxDirection = MaxSize / totalSize;
            _direction = Vector2.Min(lhs: PreferredDirection, rhs: maxDirection);
        }

        private Bounds GetBounds()
        {
            if (_elementsPositions.Count == 0) return new Bounds(center: transform.position, size: Vector3.zero);

            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            foreach (IElement element in _elementsPositions.Keys)
            {
                Bounds elementBounds = element.Bounds;
                min = Vector3.Min(lhs: min, rhs: elementBounds.min);
                max = Vector3.Max(lhs: max, rhs: elementBounds.max);
            }

            Bounds bounds = new();
            bounds.SetMinMax(min: min, max: max);
            return bounds;
        }

        // IArrangementElement
        public Transform Transform => transform;
        public Bounds Bounds => GetBounds();
    }
}
