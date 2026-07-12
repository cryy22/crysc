using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common.CoroutineControl;
using Crysc.Helpers;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public abstract class Arrangement : MonoBehaviour, ICoroutineController
    {
        public const float ZOffset = 0.01f;

        public event EventHandler<ArrangementEventArgs> ElementArrangeStarted;
        public event EventHandler<ArrangementEventArgs> ElementArrangeEnded;

        [field: SerializeField, FormerlySerializedAs("ElementsParent"), FormerlySerializedAs("ElementsParentInput")]
        public Transform ElementsParent { get; private set; }

        public virtual Vector2 ElementSize => Vector2.zero;
        public Vector2 Size { get; protected set; } = Vector2.zero;
        public Vector2 Spacing { get; protected set; } = Vector2.zero;

        [field: SerializeField] public HorizontalAlignmentType HorizontalAlignment { get; set; } =
            HorizontalAlignmentType.Left;
        [field: SerializeField] public VerticalAlignmentType VerticalAlignment { get; set; } =
            VerticalAlignmentType.Bottom;
        [field: SerializeField] public bool IsInverted { get; set; }

        [field: SerializeField, FormerlySerializedAs("OddElementStagger")]
        public Vector2 OddElementStagger { get; set; } = Vector2.zero;
        [field: SerializeField] public bool UpdateZInstantly { get; private set; } = true;

        public Coroutine ActiveCoroutine { get; set; }

        public IReadOnlyList<IElement> Elements => _elements;
        public IReadOnlyDictionary<IElement, ElementPlacement> ElementsPlacements => _elementsPlacements;
        public IReadOnlyDictionary<IElement, ElementMovementPlan> ElementsMovementPlans => _elementsMovementPlans;
        public IReadOnlyCollection<IElement> ExcludedElements => _excludedElements;

        private readonly List<IElement> _elements = new();
        private readonly Dictionary<IElement, ElementPlacement> _elementsPlacements = new();
        private readonly Dictionary<IElement, ElementMovementPlan> _elementsMovementPlans = new();
        private readonly Dictionary<IElement, Tween> _elementsTweens = new();
        private readonly HashSet<IElement> _dirtyPlanElements = new();
        private readonly HashSet<IElement> _excludedElements = new();
        private float _batchStartTime;

        private bool HasLiveTweens
        {
            get
            {
                foreach (Tween tween in _elementsTweens.Values)
                    if (tween.isAlive)
                        return true;

                return false;
            }
        }

        // time since the current batch of plans began animating; 0 when idle.
        private float AnimationTime => HasLiveTweens ? Time.time - _batchStartTime : 0f;

        public enum HorizontalAlignmentType
        {
            Left,
            Center,
            Right,
        }

        public enum VerticalAlignmentType
        {
            Top,
            Middle,
            Bottom,
        }

        public abstract void RecalculateElementPlacements();

        public void SetElements(IEnumerable<IElement> elements)
        {
            _elements.Clear();
            _elements.AddRange(elements);
            List<IElement> existingElements = _elementsPlacements.Keys.ToList();

            foreach (IElement element in _elements.Except(existingElements))
            {
                Transform eTransform = element.Transform;
                eTransform.SetParent(ElementsParent);
                eTransform.gameObject.SetActive(true);

                _elementsPlacements[element] = new ElementPlacement(
                    element: element,
                    position: eTransform.localPosition,
                    scale: eTransform.localScale,
                    rotation: eTransform.localRotation
                );
            }

            foreach (IElement element in existingElements.Except(_elements))
            {
                _elementsPlacements.Remove(element);
                _elementsMovementPlans.Remove(element);
                _dirtyPlanElements.Remove(element);
                _excludedElements.Remove(element);
                StopTweenForElement(element);
            }

            RecalculateElementPlacements();
        }

        public void AddExcludedElement(IElement element)
        {
            if (!_excludedElements.Add(element))
                return;

            RemoveMovementPlanForElement(element);
        }

        public void RemoveExcludedElement(IElement element)
        {
            _excludedElements.Remove(element);
        }

        public void SetPlacement(ElementPlacement placement)
        {
            if (!_elements.Contains(placement.Element))
            {
                Debug.Log(
                    $"Attempted to override placement for element {placement.Element.Transform.gameObject.name} which is not managed by this arrangement"
                );

                return;
            }

            _elementsPlacements[placement.Element] = placement;
        }

        public void SetMovementPlan(ElementMovementPlan plan, bool relativeTiming = true)
        {
            if (_excludedElements.Contains(plan.Element))
                return;

            float offset = relativeTiming ? AnimationTime : 0f;
            _elementsMovementPlans[plan.Element] = plan.Copy(
                startTime: plan.StartTime + offset,
                endTime: plan.EndTime + offset
            );

            _dirtyPlanElements.Add(plan.Element);
        }

        public void RemoveMovementPlanForElement(IElement element)
        {
            _elementsMovementPlans.Remove(element);
            _dirtyPlanElements.Remove(element);
            StopTweenForElement(element);
        }

        public IEnumerator ExecuteMovementPlans()
        {
            this.StartActiveCoroutine(RunMovementPlans());
            yield return this.WaitForCompletion();
        }

        public IEnumerator RunMovementPlans()
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

            if (!HasLiveTweens) _batchStartTime = Time.time;
            float elapsed = Time.time - _batchStartTime;

            foreach (IElement element in _elementsMovementPlans.Keys.ToArray())
                StartTweenForPlan(element: element, elapsed: elapsed);

            while (HasLiveTweens) yield return null;
        }

        private void StartTweenForPlan(IElement element, float elapsed)
        {
            if (_elementsTweens.TryGetValue(key: element, value: out Tween existing) && existing.isAlive)
            {
                // an untouched plan mid-flight keeps its tween; restarting it would
                // snap the element back to the plan's start pose.
                if (!_dirtyPlanElements.Contains(element)) return;
                existing.Stop();
            }

            _dirtyPlanElements.Remove(element);
            ElementMovementPlan plan = _elementsMovementPlans[element];

            _elementsTweens[element] = Tween.Custom(
                    target: this,
                    startValue: 0f,
                    endValue: 1f,
                    duration: plan.Duration,
                    startDelay: Mathf.Max(a: plan.StartTime - elapsed, b: 0f),
                    onValueChange: (arrangement, t) => arrangement.IncrementPlanTween(element: element, t: t)
                )
                .OnComplete(
                    target: this,
                    onComplete: arrangement => arrangement.CompletePlanTween(element),
                    warnIfTargetDestroyed: false
                );
        }

        private void IncrementPlanTween(IElement element, float t)
        {
            if (!_elementsMovementPlans.TryGetValue(key: element, value: out ElementMovementPlan plan)) return;

            if (!plan.IsStarted)
            {
                element.Transform.SetParent(ElementsParent);
                plan = plan.Copy(isStarted: true);
                _elementsMovementPlans[element] = plan;
                ElementArrangeStarted?.Invoke(sender: this, e: new ArrangementEventArgs(plan));
            }

            IncrementPlan(plan: plan, t: t);
        }

        private void CompletePlanTween(IElement element)
        {
            _elementsTweens.Remove(element);
            if (!_elementsMovementPlans.Remove(key: element, value: out ElementMovementPlan plan))
                return;

            ElementArrangeEnded?.Invoke(sender: this, e: new ArrangementEventArgs(plan));
        }

        private static void IncrementPlan(ElementMovementPlan plan, float t)
        {
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

        public void RearrangeInstantly()
        {
            this.StopActiveCoroutine();
            ExecuteMovementPlansInstantly();

            RecalculateElementPlacements();

            foreach (var (element, placement) in ElementsPlacements)
            {
                if (_excludedElements.Contains(element))
                    continue;

                element.Transform.SetParent(ElementsParent, false);

                element.Transform.localPosition = placement.Position;
                element.Transform.localRotation = placement.Rotation;
                element.Transform.localScale = placement.Scale;
            }
        }

        public void ExecuteMovementPlansInstantly()
        {
            this.StopActiveCoroutine();
            StopAllTweens();

            foreach (IElement element in _elementsMovementPlans.Keys)
            {
                element.Transform.localPosition = _elementsMovementPlans[element].EndPosition;
                element.Transform.localRotation = _elementsMovementPlans[element].EndRotation;
                element.Transform.localScale = _elementsMovementPlans[element].EndScale;
            }

            _elementsMovementPlans.Clear();
            _dirtyPlanElements.Clear();
        }

        private void StopTweenForElement(IElement element)
        {
            if (_elementsTweens.TryGetValue(key: element, value: out Tween tween) && tween.isAlive) tween.Stop();
            _elementsTweens.Remove(element);
        }

        private void StopAllTweens()
        {
            foreach (Tween tween in _elementsTweens.Values)
                if (tween.isAlive)
                    tween.Stop();

            _elementsTweens.Clear();
        }

        private void OnDestroy()
        {
            StopAllTweens();
        }
    }
}
