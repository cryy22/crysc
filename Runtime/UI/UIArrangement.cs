using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI
{
    using IElement = IArrangementElement;

    public class UIArrangement : MonoBehaviour, IElement
    {
        private const float _zOffset = 0.000001f;

        [SerializeField] private Transform ElementsParent;

        [SerializeField] private Vector2 BaseElementSize = Vector2.one;
        [SerializeField] private Vector2 PreferredSpaceRatio = Vector2.right;
        [SerializeField] private Vector2 OddElementStagger = Vector2.zero;
        [SerializeField] private Vector2 InitialMaxSize = Vector2.positiveInfinity;
        [SerializeField] private bool IsOrderInverted;

        private readonly Dictionary<IElement, Vector3> _elementsPositions = new();
        private Vector2 _elementSpacing;
        private Vector2 _maxSize;

        private void Awake() { _maxSize = InitialMaxSize; }

        // 3 - NON-FRONT SQUAD ELEMENTS SHOULD BE SQUEEZED INTO THE BACK

        public void InvertOrder()
        {
            IsOrderInverted = !IsOrderInverted;
            UpdateElements(_elementsPositions.Keys);
        }

        public IEnumerator AnimateUpdateMaxSize(Vector2 maxSize)
        {
            _maxSize = maxSize;
            yield return AnimateUpdateElements(_elementsPositions.Keys);
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
            UpdateElementSpacing(elements);
            var hasChanged = false;

            var index = 0;
            foreach (IElement current in elements)
            {
                if (UpdateElementPosition(current: current, index: index)) hasChanged = true;
                index++;
            }

            List<IElement> removedElements = _elementsPositions.Keys.Except(elements).ToList();
            if (removedElements.Count > 0) hasChanged = true;
            foreach (IElement element in removedElements) _elementsPositions.Remove(element);

            return hasChanged;
        }

        private bool UpdateElementPosition(IElement current, int index)
        {
            Vector3 localPosition = CalculateElementPosition(index);
            if (_elementsPositions.ContainsKey(current) && _elementsPositions[current] == localPosition) return false;

            current.Transform.SetParent(ElementsParent);
            current.Transform.gameObject.SetActive(true);
            current.Transform.localScale = Vector3.one;

            _elementsPositions[current] = localPosition;
            return true;
        }

        private Vector3 CalculateElementPosition(int index)
        {
            Vector2 localPosition2D = _elementSpacing * index;
            if (index % 2 == 1) localPosition2D += OddElementStagger;

            return new Vector3(
                x: localPosition2D.x,
                y: localPosition2D.y,
                z: _zOffset * index
            );
        }

        private void UpdateElementSpacing(IEnumerable<IElement> elements)
        {
            var directionVector = new Vector2(
                x: PreferredSpaceRatio.x > 0 ? 1 : -1,
                y: PreferredSpaceRatio.y > 0 ? 1 : -1
            );
            if (IsOrderInverted) directionVector *= -1;

            var absSpaceRatio = new Vector2(
                x: Mathf.Abs(PreferredSpaceRatio.x),
                y: Mathf.Abs(PreferredSpaceRatio.y)
            );
            Vector2 totalSize = BaseElementSize * elements.Count();
            Vector2 maxSpaceRatio = new(
                x: totalSize.x != 0 ? _maxSize.x / totalSize.x : 0,
                y: totalSize.y != 0 ? _maxSize.y / totalSize.y : 0
            );

            Vector2 percentageSpacing = Vector2.Min(lhs: absSpaceRatio, rhs: maxSpaceRatio) * directionVector;
            _elementSpacing = percentageSpacing * BaseElementSize;
        }

        // IArrangementElement
        public Transform Transform => transform;
    }
}
