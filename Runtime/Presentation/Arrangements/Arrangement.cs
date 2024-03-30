using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public class Arrangement : MonoBehaviour, IElement
    {
        public enum Alignment
        {
            Left,
            Center,
            Right,
        }

        public const float ZOffset = 0.01f;

        [FormerlySerializedAs("ElementsParent")]
        [SerializeField] private Transform ElementsParentInput;
        [FormerlySerializedAs("BaseElementSizeInput")]
        [SerializeField] public Vector2 BaseElementSize = Vector2.right; // prob won't work with a negative
        [FormerlySerializedAs("OddElementStaggerInput")]
        [SerializeField] public Vector2 OddElementStagger = Vector2.zero;
        [FormerlySerializedAs("UpdateZInstantlyInput")]
        [SerializeField] public bool UpdateZInstantly = true;

        [field: SerializeField] public Alignment HorizontalAlignment { get; set; } = Alignment.Left;
        [field: SerializeField] public bool IsInverted { get; set; }
        [field: SerializeField] public Vector2 MaxSize { get; set; } = Vector2.zero;
        [field: SerializeField] public Vector2 PreferredSpacingRatio { get; set; } = Vector2.zero;

        public IEnumerable<IElement> Elements => _elements;
        public IReadOnlyDictionary<IElement, ElementPlacement> ElementsPlacements => _elementsPlacements;
        public IReadOnlyDictionary<IElement, ElementMovementPlan> ElementsMovementPlans => _elementsMovementPlans;
        public Vector2 Direction => Vector2.one * (IsInverted ? -1 : 1);

        public Vector2 AlignmentOffset { get; private set; }
        public Vector2 Spacing { get; private set; } = Vector2.zero;

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 Pivot { get; private set; } = new(x: 0.5f, y: 0.5f);
        public Vector2 SizeMultiplier { get; private set; } = Vector2.zero;
        public bool IsAnimating => _mainRearrangeRoutine is { IsComplete: false };

        public Action PreElementArrangeHook { get; set; }
        public Action PostElementArrangeHook { get; set; }

        private readonly List<IElement> _elements = new();
        private readonly Dictionary<IElement, ElementPlacement> _elementsPlacements = new();
        private readonly Dictionary<IElement, float> _elementsDistances = new();
        private readonly Dictionary<IElement, ElementMovementPlan> _elementsMovementPlans = new();
        private readonly HashSet<IElement> _excludedFromRearrange = new();

        private CryRoutine _mainRearrangeRoutine;
        private IArrangementCalculator _arrangementCalculator;

        private float _animationTime;
        private CryRoutine _animationRoutine;

        private void Awake()
        {
            if (BaseElementSize.x < 0 || BaseElementSize.y < 0)
                throw new Exception("BaseElementSize cannot be negative");
            _arrangementCalculator = GetComponent<IArrangementCalculator>() ?? new DefaultArrangementCalculator();
        }

        public void SetElements(IEnumerable<IElement> elements)
        {
            _elements.Clear();
            _elements.AddRange(elements);
            List<IElement> existingElements = _elementsPlacements.Keys.ToList();

            _excludedFromRearrange.IntersectWith(_elements);
            foreach (IElement element in _elements.Except(existingElements)) AddElement(element);
            foreach (IElement element in existingElements.Except(_elements)) _elementsPlacements.Remove(element);

            UpdateElementsAndPositions();
        }

        public void SetMovementPlans(IEnumerable<ElementMovementPlan> plans)
        {
            foreach (ElementMovementPlan plan in plans)
            {
                if (!plan.RequiresMovement) continue;

                _elementsMovementPlans[plan.Element] = plan.Copy(
                    startTime: plan.StartTime + _animationTime,
                    endTime: plan.EndTime + _animationTime
                );
            }
        }

        public IEnumerator ExecuteMovementPlans()
        {
            _animationRoutine?.Stop();
            _animationRoutine = new CryRoutine(enumerator: Run(), behaviour: this);
            yield return _animationRoutine.WaitForCompletion();
            yield break;

            IEnumerator Run()
            {
                var expiredPlans = new HashSet<ElementMovementPlan>();
                while (_elementsMovementPlans.Count > 0)
                {
                    foreach (ElementMovementPlan plan in _elementsMovementPlans.Values)
                    {
                        IncrementPlan(plan: plan, time: _animationTime);
                        if (_animationTime > plan.EndTime) expiredPlans.Add(plan);
                    }

                    foreach (ElementMovementPlan plan in expiredPlans)
                        _elementsMovementPlans.Remove(plan.Element);
                    expiredPlans.Clear();

                    yield return null;
                    _animationTime += Time.deltaTime;
                }

                _animationTime = 0f;
            }
        }

        public void Rearrange()
        {
            _mainRearrangeRoutine?.Stop();

            UpdateElementsAndPositions();
            foreach (IElement element in _elementsPlacements.Keys.Except(_excludedFromRearrange))
            {
                element.Transform.localPosition = _elementsPlacements[element].Position;
                element.Transform.localRotation = _elementsPlacements[element].Rotation;
                element.Transform.localScale = Vector3.one;
            }
        }

        public IEnumerator AnimateRearrange(float duration, float? perElementDelay = null)
        {
            _mainRearrangeRoutine?.Stop();
            _elementsDistances.Clear();
            _elementsMovementPlans.Clear();

            UpdateElementsAndPositions();
            IElement[] elements = _elementsPlacements.Keys.Except(_excludedFromRearrange).ToArray();
            if (elements.Length == 0) yield break;
            UpdateElementsMovementPlans(elements: elements, totalDuration: duration);

            if (UpdateZInstantly)
                foreach (IElement e in elements)
                {
                    Transform elementTransform = e.Transform;
                    e.Transform.localPosition = new Vector3(
                        x: elementTransform.localPosition.x,
                        y: elementTransform.localPosition.y,
                        z: _elementsPlacements[e].Position.z
                    );
                }

            _mainRearrangeRoutine = new CryRoutine(enumerator: Run(), behaviour: this);
            yield return _mainRearrangeRoutine.WaitForCompletion();

            yield break;

            IEnumerator Run()
            {
                float trueDuration = _elementsMovementPlans[elements.Last()].EndTime;

                var time = 0f;
                while (time <= trueDuration)
                {
                    yield return null;
                    time += Time.deltaTime;

                    foreach (IElement e in elements)
                    {
                        ElementMovementPlan plan = _elementsMovementPlans[e];

                        if (plan.IsCompleted) continue;
                        if (time < plan.StartTime) continue;
                        if (!plan.IsBegun)
                        {
                            PreElementArrangeHook?.Invoke();
                            _elementsMovementPlans[e] = plan.Copy(isBegun: true);
                        }

                        IncrementPlan(plan: plan, time: time);

                        if (time > plan.EndTime)
                        {
                            PostElementArrangeHook?.Invoke();
                            _elementsMovementPlans[e] = plan.Copy(isCompleted: true);
                        }
                    }
                }
            }
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

            Vector2 size = totalSize + (Spacing * (_elements.Count - 1));

            SizeMultiplier = new Vector2(
                x: BaseElementSize.x > 0 ? size.x / BaseElementSize.x : 0,
                y: BaseElementSize.y > 0 ? size.y / BaseElementSize.y : 0
            );
            AlignmentOffset = HorizontalAlignment switch
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

        public static void IncrementPlan(ElementMovementPlan plan, float time)
        {
            float t = Mathf.Clamp01((time - plan.StartTime) / plan.Duration);

            Mover.MoveToStep(
                transform: plan.Element.Transform,
                start: plan.StartPosition,
                end: plan.EndPosition,
                t: t,
                easing: plan.Easing
            );
            Rotator.RotateToStep(
                transform: plan.Element.Transform,
                start: plan.StartRotation,
                end: plan.EndRotation,
                t: t,
                rotations: plan.ExtraRotations,
                easings: plan.Easing
            );
            Scaler.ScaleToStep(
                transform: plan.Element.Transform,
                start: plan.StartScale,
                end: plan.EndScale,
                t: t,
                easing: plan.Easing
            );
        }

        private void UpdateElementsAndPositions()
        {
            UpdateProperties();
            foreach (ElementPlacement placement in _arrangementCalculator.CalculateElementPlacements(this))
                _elementsPlacements[placement.Element] = placement;
        }

        private void UpdateElementsMovementPlans(IElement[] elements, float totalDuration)
        {
            foreach (IElement element in elements)
                _elementsDistances[element] = Vector2.Distance(
                    a: element.Transform.localPosition,
                    b: _elementsPlacements[element].Position
                );

            float overlapAwareDistance =
                elements.Select(e => _elementsDistances[e] * 0.5f).Sum() +
                (_elementsDistances[elements.First()] * 0.5f);
            overlapAwareDistance = Mathf.Max(a: overlapAwareDistance, b: Mathf.Epsilon);

            var startTime = 0f;
            var endTime = 0f;
            foreach (IElement e in elements)
            {
                float duration = (_elementsDistances[e] / overlapAwareDistance) * totalDuration;
                startTime = Mathf.Max(
                    a: endTime - (0.5f * duration),
                    b: startTime
                );
                endTime = startTime + duration;

                _elementsMovementPlans[e] = ArrangementMovementScheduler.CreateMovementPlan(
                    arrangement: this,
                    element: e,
                    startTime: startTime,
                    endTime: endTime,
                    extraRotations: 0,
                    easing: Easings.Enum.Linear
                );
            }
        }

        private void AddElement(IElement element)
        {
            Transform elementTransform = element.Transform;
            elementTransform.SetParent(ElementsParentInput);
            elementTransform.gameObject.SetActive(true);

            _elementsPlacements[element] =
                new ElementPlacement(
                    element: element,
                    position: elementTransform.localPosition,
                    rotation: elementTransform.localRotation
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

            Spacing = Vector2.Min(lhs: maxSpacing, rhs: preferredSpacing);
        }
    }
}
