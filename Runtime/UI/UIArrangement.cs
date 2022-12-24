using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI
{
    using IElement = IArrangementElement;

    public class UIArrangement : MonoBehaviour, IArrangement<IElement>, IElement
    {
        private const float _zOffset = 0.000001f;

        [SerializeField] private Transform ElementsParent;

        [SerializeField] private Vector2 BaseElementSize = Vector2.right; // prob won't work with a negative
        [SerializeField] private Vector2 OddElementStagger = Vector2.zero;

        private readonly Dictionary<IElement, Vector3> _elementsPositions = new();
        private readonly List<IElement> _elements = new();
        private Vector2 _spacing = Vector2.zero;
        private Vector2 _centeringOffset;

        private Vector2 Direction => Vector2.one * (IsInverted ? -1 : 1);

        private void Awake()
        {
            if (BaseElementSize.x < 0 || BaseElementSize.y < 0)
                throw new Exception("BaseElementSize cannot be negative");
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
            _centeringOffset = IsCentered ? size / 2 : Vector2.zero;

            if (IsCentered) Pivot = new Vector2(x: 0.5f, y: 0.5f);
            else if (IsInverted) Pivot = new Vector2(x: 1, y: 1);
            else Pivot = Vector2.zero;
        }

        private void UpdateElementsAndPositions()
        {
            UpdateProperties();

            Vector2 weightedIndexes = Vector2.zero;
            var index = 0;
            foreach (IElement element in _elements)
            {
                Vector3 startPoint = CalculateElementStartPoint(weightedIndexes: weightedIndexes, index: index);
                _elementsPositions[element] = CalculateElementAnchorPoint(element: element, startPoint: startPoint);

                weightedIndexes += element.SizeMultiplier;
                index++;
            }
        }

        private void AddElement(IElement element)
        {
            Transform elementTransform = element.Transform;
            elementTransform.SetParent(ElementsParent);
            elementTransform.gameObject.SetActive(true);
            elementTransform.localScale = Vector3.one;

            _elementsPositions[element] = elementTransform.localPosition;
        }

        private Vector3 CalculateElementStartPoint(Vector2 weightedIndexes, int index)
        {
            Vector2 startPoint2d = BaseElementSize * weightedIndexes;
            startPoint2d += _spacing * index;
            startPoint2d -= _centeringOffset;
            if (index % 2 == 1) startPoint2d += OddElementStagger;
            startPoint2d *= Direction;

            return new Vector3(
                x: startPoint2d.x,
                y: startPoint2d.y,
                z: _zOffset * index
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

            return new Vector3(
                x: midpoint2d.x,
                y: midpoint2d.y,
                z: startPoint.z
            );
        }

        // IArrangement
        [field: SerializeField] public bool IsCentered { get; set; }
        [field: SerializeField] public bool IsInverted { get; set; }
        [field: SerializeField] public Vector2 MaxSize { get; set; } = Vector2.zero;
        [field: SerializeField] public Vector2 PreferredSpacingRatio { get; set; } = Vector2.zero;

        public void SetElements(IEnumerable<IElement> elements)
        {
            _elements.Clear();
            _elements.AddRange(elements);
            List<IElement> existingElements = _elementsPositions.Keys.ToList();

            foreach (IElement element in _elements.Except(existingElements)) AddElement(element);
            foreach (IElement element in existingElements.Except(_elements)) _elementsPositions.Remove(element);
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
        public Vector2 Pivot { get; private set; } = new(x: 0.5f, y: 0.5f);
        public Vector2 SizeMultiplier { get; private set; } = Vector2.zero;
    }
}
