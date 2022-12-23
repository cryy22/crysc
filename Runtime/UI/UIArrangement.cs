using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI
{
    using IElement = IArrangementElement;

    public class UIArrangement : MonoBehaviour, IArrangement, IElement
    {
        private const float _zOffset = 0.000001f;

        [SerializeField] private Transform ElementsParent;

        [SerializeField] private Vector2 BaseElementSpacing = Vector2.right; // prob won't work with a negative
        [SerializeField] private Vector2 PreferredOverhangRatio = Vector2.zero;
        [SerializeField] private Vector2 OddElementStagger = Vector2.zero;

        private readonly Dictionary<IElement, Vector3> _elementsPositions = new();
        private Vector2 _overhang;
        private Vector2 _centeringOffset;

        private Vector2 ElementSpacing => BaseElementSpacing * (IsInverted ? -1 : 1);

        public void UpdateElements(IEnumerable<IElement> elements)
        {
            SetElements(elements);
            Rearrange();
        }

        public IEnumerator AnimateUpdateElements(IEnumerable<IElement> elements, float duration = 0.25f)
        {
            SetElements(elements);
            yield return AnimateRearrange(duration);
        }

        private void UpdateElementsAndPositions()
        {
            List<IElement> elements = _elementsPositions.Keys.ToList();

            UpdateOverhang(elements);

            Vector2 weightedIndexes = Vector2.zero;
            var index = 0;
            foreach (IElement element in elements)
            {
                Vector3 startPoint = CalculateElementStartPoint(weightedIndexes: weightedIndexes, index: index);
                Vector3 localPosition = CalculateElementTransformPosition(element: element, startPoint: startPoint);

                UpdateElementPosition(element: element, localPosition: localPosition);

                weightedIndexes += element.SpacingMultiplier;
                index++;
            }
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
            Vector2 startPoint2d = (ElementSpacing * weightedIndexes) - (_overhang * index) - _centeringOffset;
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

            SpacingMultiplier = totalSizeUnits - (overhangRatio * (elements.Count() - 1));
            _centeringOffset = IsCentered ? (SpacingMultiplier * ElementSpacing) / 2 : Vector2.zero;
        }

        private Vector2 CalculateMinOverhangRatio(Vector2 totalSizeUnits, int count)
        {
            if (count <= 1) return Vector2.negativeInfinity;

            Vector2 maxSizeUnits = MaxSize / BaseElementSpacing;
            return (totalSizeUnits - maxSizeUnits) / (count - 1);
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

        // IArrangement
        [field: SerializeField] public bool IsCentered { get; set; }
        [field: SerializeField] public bool IsInverted { get; set; }
        [field: SerializeField] public Vector2 MaxSize { get; set; } = Vector2.positiveInfinity;

        public void SetElements(IEnumerable<IElement> elements)
        {
            elements = elements.ToList();
            List<IElement> existingElements = _elementsPositions.Keys.ToList();

            foreach (IElement element in elements.Except(existingElements))
                _elementsPositions[element] = element.Transform.localPosition;
            foreach (IElement element in existingElements.Except(elements))
                _elementsPositions.Remove(element);
        }

        public void Rearrange()
        {
            UpdateElementsAndPositions();
            foreach (IElement element in _elementsPositions.Keys)
                element.Transform.localPosition = _elementsPositions[element];
        }

        public IEnumerator AnimateRearrange(float duration = 0.25f)
        {
            UpdateElementsAndPositions();

            Coroutine[] coroutines = _elementsPositions
                .Select(
                    kvp => StartCoroutine(
                        Mover.MoveLocal(transform: kvp.Key.Transform, end: kvp.Value, duration: duration)
                    )
                )
                .ToArray();

            yield return CoroutineWaiter.RunConcurrently(coroutines);
        }

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 Pivot => IsCentered ? Vector2.one * 0.5f : Vector2.zero;
        public Vector2 SpacingMultiplier { get; private set; } = Vector2.one;
    }
}
