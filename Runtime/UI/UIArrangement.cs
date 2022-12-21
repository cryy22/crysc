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

        [SerializeField] private Vector2 BaseElementSpacing = Vector2.right; // prob won't work with a negative
        [SerializeField] private Vector2 PreferredOverhangRatio = Vector2.zero;
        [SerializeField] private Vector2 OddElementStagger = Vector2.zero;
        [SerializeField] private Vector2 InitialMaxSize = Vector2.positiveInfinity;
        [SerializeField] private bool IsOrderInverted;

        private readonly Dictionary<IElement, Vector3> _elementsPositions = new();
        private Vector2 _overhang;
        private Vector2 _maxSizeUnits;

        private Vector2 ElementSpacing => BaseElementSpacing * (IsOrderInverted ? -1 : 1);

        private void Awake() { _maxSizeUnits = InitialMaxSize / BaseElementSpacing; }

        // 3 - NON-FRONT SQUAD ELEMENTS SHOULD BE SQUEEZED INTO THE BACK

        public void InvertOrder()
        {
            IsOrderInverted = !IsOrderInverted;
            UpdateElements(_elementsPositions.Keys);
        }

        public IEnumerator AnimateUpdateMaxSize(Vector2 maxSize)
        {
            _maxSizeUnits = maxSize / BaseElementSpacing;
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
            UpdateOverhang(elements);
            var hasChanged = false;

            Vector2 weightedIndexes = Vector2.zero;
            var index = 0;
            foreach (IElement element in elements)
            {
                Vector3 startPoint = CalculateElementStartPoint(weightedIndexes: weightedIndexes, index: index);
                Vector3 localPosition = CalculateElementTransformPosition(element: element, startPoint: startPoint);

                if (UpdateElementPosition(element: element, localPosition: localPosition)) hasChanged = true;

                weightedIndexes += element.SpacingMultiplier;
                index++;
            }

            List<IElement> removedElements = _elementsPositions.Keys.Except(elements).ToList();
            if (removedElements.Count > 0) hasChanged = true;
            foreach (IElement element in removedElements) _elementsPositions.Remove(element);

            return hasChanged;
        }

        private bool UpdateElementPosition(IElement element, Vector3 localPosition)
        {
            if (_elementsPositions.ContainsKey(element) && _elementsPositions[element] == localPosition) return false;

            element.Transform.SetParent(ElementsParent);
            element.Transform.gameObject.SetActive(true);
            element.Transform.localScale = Vector3.one;

            _elementsPositions[element] = localPosition;
            return true;
        }

        private Vector3 CalculateElementStartPoint(Vector2 weightedIndexes, int index)
        {
            Vector2 startPoint2d = (ElementSpacing * weightedIndexes) - (_overhang * index);
            if (index % 2 == 1) startPoint2d += OddElementStagger;

            return new Vector3(
                x: startPoint2d.x,
                y: startPoint2d.y,
                z: _zOffset * index
            );
        }

        private void UpdateOverhang(IEnumerable<IElement> elements)
        {
            elements = elements.ToList();
            Vector2 totalSizeUnits = elements.Aggregate(
                seed: Vector2.zero,
                (acc, e) => acc + e.SpacingMultiplier
            );

            Vector2 minOverhangRatio = CalculateMinOverhangRatio(
                totalSizeUnits: totalSizeUnits,
                count: elements.Count()
            );

            Vector2 overhangRatio = Vector2.Max(lhs: PreferredOverhangRatio, rhs: minOverhangRatio);
            _overhang = overhangRatio * ElementSpacing;
        }

        private Vector2 CalculateMinOverhangRatio(Vector2 totalSizeUnits, int count)
        {
            if (count <= 1) return Vector2.negativeInfinity;
            return (totalSizeUnits - _maxSizeUnits) / (count - 1);
        }

        private Vector3 CalculateElementTransformPosition(IElement element, Vector3 startPoint)
        {
            Vector2 elementSpacing = ElementSpacing * element.SpacingMultiplier;
            Vector2 midpoint2d = (Vector2) startPoint + (elementSpacing * element.Pivot);

            return new Vector3(
                x: midpoint2d.x,
                y: midpoint2d.y,
                z: startPoint.z
            );
        }

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 SpacingMultiplier => Vector2.one;
        public Vector2 Pivot => Vector2.zero;
    }
}
