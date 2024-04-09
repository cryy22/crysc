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

        public event EventHandler ElementArrangeStarted;
        public event EventHandler ElementArrangeEnded;

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

        public IReadOnlyList<IElement> Elements => _elements;
        public IReadOnlyDictionary<IElement, ElementPlacement> ElementsPlacements => _elementsPlacements;
        public IReadOnlyDictionary<IElement, ElementMovementPlan> ElementsMovementPlans => _elementsMovementPlans;
        public Vector2 Direction => Vector2.one * (IsInverted ? -1 : 1);

        public Vector2 AlignmentOffset { get; private set; }
        public Vector2 Spacing { get; private set; } = Vector2.zero;

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 Pivot { get; private set; } = new(x: 0.5f, y: 0.5f);
        public Vector2 SizeMultiplier { get; private set; } = Vector2.zero;

        private readonly List<IElement> _elements = new();
        private readonly Dictionary<IElement, ElementPlacement> _elementsPlacements = new();
        private readonly Dictionary<IElement, ElementMovementPlan> _elementsMovementPlans = new();

        private IArrangementCalculator _arrangementCalculator;
        private float _animationTime;
        private CryRoutine _movementPlanRoutine;

        private void Awake()
        {
            if ((BaseElementSize.x < 0) || (BaseElementSize.y < 0))
                throw new Exception("BaseElementSize cannot be negative");
            _arrangementCalculator = GetComponent<IArrangementCalculator>() ?? new DefaultArrangementCalculator();
        }

        public void SetElements(IEnumerable<IElement> elements)
        {
            _elements.Clear();
            _elements.AddRange(elements);
            List<IElement> existingElements = _elementsPlacements.Keys.ToList();

            foreach (IElement element in _elements.Except(existingElements))
            {
                Transform eTransform = element.Transform;
                eTransform.SetParent(ElementsParentInput);
                eTransform.gameObject.SetActive(true);

                _elementsPlacements[element] = new ElementPlacement(
                    element: element,
                    position: eTransform.localPosition,
                    rotation: eTransform.localRotation
                );
            }

            foreach (IElement element in existingElements.Except(_elements))
            {
                _elementsPlacements.Remove(element);
                _elementsMovementPlans.Remove(element);
            }

            RecalculateElementPlacements();
        }

        public void SetMovementPlan(ElementMovementPlan plan, bool relativeTiming = true)
        {
            _elementsMovementPlans[plan.Element] = plan.Copy(
                startTime: plan.StartTime + (relativeTiming ? _animationTime : 0),
                endTime: plan.EndTime + (relativeTiming ? _animationTime : 0)
            );
        }

        public void RemoveMovementPlanForElement(IElement element) { _elementsMovementPlans.Remove(element); }

        public IEnumerator ExecuteMovementPlans()
        {
            _movementPlanRoutine?.Stop();

            _movementPlanRoutine = new CryRoutine(enumerator: Run(), behaviour: this);
            yield return _movementPlanRoutine.WaitForCompletion();
            yield break;

            IEnumerator Run()
            {
                if (UpdateZInstantly)
                    foreach (IElement element in _elementsMovementPlans.Keys.ToArray())
                    {
                        ElementMovementPlan plan = _elementsMovementPlans[element];
                        Vector3 currentPosition = plan.Element.Transform.localPosition;
                        plan.Element.Transform.localPosition = new Vector3(
                            x: currentPosition.x,
                            y: currentPosition.y,
                            z: plan.EndPosition.z
                        );

                        _elementsMovementPlans[element] = plan.Copy(
                            startPosition: new Vector3(
                                x: plan.StartPosition.x,
                                y: plan.StartPosition.y,
                                z: plan.EndPosition.z
                            )
                        );
                    }

                var startedPlans = new HashSet<ElementMovementPlan>();
                var endedPlans = new HashSet<ElementMovementPlan>();

                while (_elementsMovementPlans.Count > 0)
                {
                    foreach (ElementMovementPlan plan in _elementsMovementPlans.Values)
                    {
                        if (!plan.IsStarted)
                            if (_animationTime > plan.StartTime) startedPlans.Add(plan);
                            else continue;

                        IncrementPlan(plan: plan, time: _animationTime);

                        if (_animationTime > plan.EndTime)
                            endedPlans.Add(plan);
                    }

                    foreach (ElementMovementPlan plan in startedPlans)
                    {
                        _elementsMovementPlans[plan.Element] = plan.Copy(isStarted: true);
                        ElementArrangeStarted?.Invoke(sender: this, e: EventArgs.Empty);
                    }

                    foreach (ElementMovementPlan plan in endedPlans)
                    {
                        _elementsMovementPlans.Remove(plan.Element);
                        ElementArrangeEnded?.Invoke(sender: this, e: EventArgs.Empty);
                    }

                    startedPlans.Clear();
                    endedPlans.Clear();

                    yield return null;
                    _animationTime += Time.deltaTime;
                }

                _animationTime = 0f;
            }
        }

        public void RearrangeInstantly()
        {
            _movementPlanRoutine?.Stop();

            foreach (IElement element in _elementsMovementPlans.Keys)
            {
                element.Transform.localPosition = _elementsMovementPlans[element].EndPosition;
                element.Transform.localRotation = _elementsMovementPlans[element].EndRotation;
                element.Transform.localScale = _elementsMovementPlans[element].EndScale;
            }

            IEnumerable<IElement> remainingElements = _elements.Except(_elementsMovementPlans.Keys).ToArray();
            _elementsMovementPlans.Clear();

            if (!remainingElements.Any()) return;

            RecalculateElementPlacements();
            foreach (IElement element in remainingElements)
            {
                element.Transform.localPosition = _elementsPlacements[element].Position;
                element.Transform.localRotation = _elementsPlacements[element].Rotation;
                element.Transform.localScale = Vector3.one;
            }
        }

        public void UpdateProperties()
        {
            Vector2 totalSize = _elements.Aggregate(
                seed: Vector2.zero,
                (acc, e) => acc + e.SizeMultiplier
            ) * BaseElementSize;

            if (_elements.Count > 1)
            {
                var maxSize = new Vector2(
                    x: MaxSize.x > 0 ? MaxSize.x : float.PositiveInfinity,
                    y: MaxSize.y > 0 ? MaxSize.y : float.PositiveInfinity
                );
                Vector2 maxSpacing = (maxSize - totalSize) / (_elements.Count - 1);
                Vector2 preferredSpacing = PreferredSpacingRatio * BaseElementSize;

                Spacing = Vector2.Min(lhs: maxSpacing, rhs: preferredSpacing);
            }

            Vector2 size = totalSize + Spacing * (_elements.Count - 1);

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

        public void RecalculateElementPlacements()
        {
            UpdateProperties();
            foreach (ElementPlacement placement in _arrangementCalculator.CalculateElementPlacements(this))
                _elementsPlacements[placement.Element] = placement;
        }
    }
}
