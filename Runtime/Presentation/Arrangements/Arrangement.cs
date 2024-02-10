using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public class Arrangement : MonoBehaviour, IArrangement, IElement
    {
        public enum Alignment
        {
            Left,
            Center,
            Right,
        }

        public const float ZOffset = 0.01f;

        [SerializeField] private Transform ElementsParent;
        [SerializeField] private Vector2 BaseElementSize = Vector2.right; // prob won't work with a negative
        [SerializeField] public Vector2 OddElementStagger = Vector2.zero;
        [SerializeField] private bool UpdateZInstantly = true;

        [SerializeField] private SplineArrangementCalculator SplineArrangementCalculator;

        // IArrangement
        [field: SerializeField] public Alignment HorizontalAlignment { get; set; } = Alignment.Left;
        [field: SerializeField] public bool IsInverted { get; set; }
        [field: SerializeField] public Vector2 MaxSize { get; set; } = Vector2.zero;
        [field: SerializeField] public Vector2 PreferredSpacingRatio { get; set; } = Vector2.zero;

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 Pivot { get; private set; } = new(x: 0.5f, y: 0.5f);
        public Vector2 SizeMultiplier { get; private set; } = Vector2.zero;

        private readonly List<IElement> _elements = new();
        private readonly Dictionary<IElement, ElementPlacement> _elementsPlacements = new();
        private readonly HashSet<IElement> _excludedFromRearrange = new();
        private readonly List<ConcurrentCryRoutine> _rearrangeRoutines = new();

        private Vector2 _spacing = Vector2.zero;
        private Vector2 _alignmentOffset;

        private Vector2 Direction => Vector2.one * (IsInverted ? -1 : 1);

        private void Awake()
        {
            if (BaseElementSize.x < 0 || BaseElementSize.y < 0)
                throw new Exception("BaseElementSize cannot be negative");
        }

        public void SetElements(IEnumerable<IElement> elements)
        {
            _elements.Clear();
            _elements.AddRange(elements);
            List<IElement> existingElements = _elementsPlacements.Keys.ToList();

            _excludedFromRearrange.IntersectWith(_elements);
            foreach (IElement element in _elements.Except(existingElements)) AddElement(element);
            foreach (IElement element in existingElements.Except(_elements)) _elementsPlacements.Remove(element);
        }

        public void Rearrange()
        {
            UpdateElementsAndPositions();
            foreach (IElement element in _elementsPlacements.Keys.Except(_excludedFromRearrange))
            {
                element.Transform.localPosition = _elementsPlacements[element].Position;
                element.Transform.localScale = Vector3.one;
            }
        }

        public IEnumerator AnimateRearrange(float duration) { return ((IArrangement) this).AnimateRearrange(duration); }

        public IEnumerator AnimateRearrange(float duration, float perElementDelay)
        {
            UpdateElementsAndPositions();

            foreach (ConcurrentCryRoutine routine in _rearrangeRoutines) routine.Stop();
            _rearrangeRoutines.Clear();

            IElement[] elements = _elementsPlacements.Keys.Except(_excludedFromRearrange).ToArray();
            float perElementDuration = Mathf.Max(a: 0, b: duration - (perElementDelay * (elements.Length - 1)));
            foreach (IElement e in elements)
            {
                _rearrangeRoutines.Add(AnimateElementPlacement(e: e, duration: perElementDuration));
                yield return new WaitForSeconds(perElementDelay);
            }

            yield return CoroutineWaiter.RunConcurrently(_rearrangeRoutines.ToArray());
        }

        public int GetClosestIndex(Vector2 position, bool isLocal = true)
        {
            var closestIndex = 0;
            var closestDistance = float.MaxValue;
            Vector2 localPosition = isLocal ? position : transform.InverseTransformPoint(position);

            for (var i = 0; i < _elements.Count; i++)
            {
                if (!_elementsPlacements.TryGetValue(key: _elements[i], value: out ElementPlacement placement))
                    continue;
                float distance = Vector2.Distance(a: localPosition, b: placement.Position);

                if (!(distance < closestDistance)) continue;

                closestDistance = distance;
                closestIndex = i;
            }

            return closestIndex;
        }

        public int GetInsertionIndex(Vector2 position, bool isLocal = true, bool useXAxis = true)
        {
            Vector2 localPosition = isLocal ? position : transform.InverseTransformPoint(position);

            int closestIndex = GetClosestIndex(position: localPosition);
            IElement closestElement = _elements.ElementAtOrDefault(closestIndex);

            if (closestElement == null) return closestIndex;
            if (!_elementsPlacements.TryGetValue(key: closestElement, value: out ElementPlacement placement))
                return closestIndex;

            float closestAxialPosition =
                (useXAxis ? placement.Position.x : placement.Position.y) * (IsInverted ? -1 : 1);
            float axialPosition = (useXAxis ? localPosition.x : localPosition.y) * (IsInverted ? -1 : 1);

            return axialPosition < closestAxialPosition ? closestIndex : closestIndex + 1;
        }

        public void UpdateProperties()
        {
            Vector2 totalSize = _elements.Aggregate(
                seed: Vector2.zero,
                (acc, e) => acc + e.SizeMultiplier
            ) * BaseElementSize;

            UpdateSpacing(totalSize: totalSize, count: _elements.Count);

            Vector2 size = totalSize + (_spacing * (_elements.Count - 1));

            SizeMultiplier = new Vector2(
                x: BaseElementSize.x > 0 ? size.x / BaseElementSize.x : 0,
                y: BaseElementSize.y > 0 ? size.y / BaseElementSize.y : 0
            );
            _alignmentOffset = HorizontalAlignment switch
            {
                Alignment.Left   => Vector2.zero,
                Alignment.Center => size / 2,
                Alignment.Right  => size,
                _                => throw new ArgumentOutOfRangeException(),
            };

            Pivot = HorizontalAlignment switch
            {
                Alignment.Left   => IsInverted ? Vector2.one : Vector2.zero,
                Alignment.Center => new Vector2(x: 0.5f, y: 0.5f),
                Alignment.Right  => IsInverted ? Vector2.zero : Vector2.one,
                _                => throw new ArgumentOutOfRangeException(),
            };
        }

        public void ExcludeFromRearrange(IElement element) { _excludedFromRearrange.Add(item: element); }
        public void IncludeInRearrange(IElement element) { _excludedFromRearrange.Remove(item: element); }

        private ConcurrentCryRoutine AnimateElementPlacement(IElement e, float duration)
        {
            Transform elementTransform = e.Transform;
            if (UpdateZInstantly)
                e.Transform.localPosition = new Vector3(
                    x: elementTransform.localPosition.x,
                    y: elementTransform.localPosition.y,
                    z: _elementsPlacements[e].Position.z
                );

            return new ConcurrentCryRoutine(
                behaviour: this,
                Mover.MoveToSmoothly(
                    transform: elementTransform,
                    end: _elementsPlacements[e].Position,
                    duration: duration
                ),
                Rotator.RotateToSmoothly(
                    transform: elementTransform,
                    end: _elementsPlacements[e].Rotation,
                    duration: duration
                ),
                Scaler.ScaleToSmoothly(transform: elementTransform, end: Vector3.one, duration: duration)
            );
        }

        private void UpdateElementsAndPositions()
        {
            UpdateProperties();
            if (SplineArrangementCalculator)
            {
                foreach (
                    ElementPlacement placement in SplineArrangementCalculator.CalculateElementPlacements(
                        elements: _elements,
                        elementWidth: BaseElementSize.x,
                        preferredSpacingRatio: PreferredSpacingRatio.x
                    )
                )
                    _elementsPlacements[placement.Element] = placement;
                return;
            }


            Vector2 weightedIndexes = Vector2.zero;
            var index = 0;
            foreach (IElement element in _elements)
            {
                Vector3 startPoint = CalculateElementStartPoint(weightedIndexes: weightedIndexes, index: index);
                _elementsPlacements[element] = new ElementPlacement(
                    element: element,
                    position: CalculateElementAnchorPoint(element: element, startPoint: startPoint),
                    rotation: Quaternion.identity
                );

                weightedIndexes += element.SizeMultiplier;
                index++;
            }
        }

        private void AddElement(IElement element)
        {
            Transform elementTransform = element.Transform;
            elementTransform.SetParent(ElementsParent);
            elementTransform.gameObject.SetActive(true);

            _elementsPlacements[element] =
                new ElementPlacement(
                    element: element,
                    position: elementTransform.localPosition,
                    rotation: elementTransform.localRotation
                );
        }

        private Vector3 CalculateElementStartPoint(Vector2 weightedIndexes, int index)
        {
            Vector2 startPoint2d = BaseElementSize * weightedIndexes;
            startPoint2d += _spacing * index;
            startPoint2d -= _alignmentOffset;
            if (index % 2 == 1) startPoint2d += OddElementStagger;
            startPoint2d *= Direction;

            return new Vector3(
                x: startPoint2d.x,
                y: startPoint2d.y,
                z: ZOffset * index
            );
        }

        private void UpdateSpacing(Vector2 totalSize, int count)
        {
            if (count <= 1) return;

            var maxSize = new Vector2(
                x: MaxSize.x > 0 ? MaxSize.x : float.PositiveInfinity,
                y: MaxSize.y > 0 ? MaxSize.y : float.PositiveInfinity
            );
            Vector2 maxSpacing = (maxSize - totalSize) / (count - 1);
            Vector2 preferredSpacing = PreferredSpacingRatio * BaseElementSize;

            _spacing = Vector2.Min(lhs: maxSpacing, rhs: preferredSpacing);
        }

        private Vector3 CalculateElementAnchorPoint(IElement element, Vector3 startPoint)
        {
            Vector2 elementSize = BaseElementSize * element.SizeMultiplier;
            Vector2 directionalPivot = element.Pivot - (IsInverted ? Vector2.one : Vector2.zero);
            Vector2 midpoint2d = (Vector2) startPoint + (elementSize * directionalPivot);

            return new Vector3(x: midpoint2d.x, y: midpoint2d.y, z: startPoint.z) + element.ArrangementOffset;
        }
    }
}
