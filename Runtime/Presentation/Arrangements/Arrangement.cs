using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common.CoroutineControl;
using Crysc.Helpers;
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
        public Vector2 Spacing { get; protected set; } = Vector2.zero;
        public virtual Vector2 AlignmentOffset => Vector2.zero;
        [field: SerializeField] public bool IsInverted { get; set; }
        public Vector2 Direction => Vector2.one * (IsInverted ? -1 : 1);
        [SerializeField] public Vector2 OddElementStagger = Vector2.zero;
        [field: SerializeField] public bool UpdateZInstantly { get; private set; } = true;
        
        public Coroutine ActiveCoroutine { get; set; }

        public IReadOnlyList<IElement> Elements => _elements;
        public IReadOnlyDictionary<IElement, ElementPlacement> ElementsPlacements => _elementsPlacements;
        public IReadOnlyDictionary<IElement, ElementMovementPlan> ElementsMovementPlans => _elementsMovementPlans;
        
        protected readonly List<IElement> _elements = new();
        protected readonly Dictionary<IElement, ElementPlacement> _elementsPlacements = new();
        protected readonly Dictionary<IElement, ElementMovementPlan> _elementsMovementPlans = new();
        private float _animationTime;
        
        public enum Alignment
        {
            Left,
            Center,
            Right,
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

            var startedPlans = new HashSet<ElementMovementPlan>();
            var endedPlans = new HashSet<ElementMovementPlan>();

            while (_elementsMovementPlans.Count > 0)
            {
                foreach (ElementMovementPlan plan in _elementsMovementPlans.Values)
                {
                    if (!plan.IsStarted)
                        if (_animationTime > plan.StartTime)
                        {
                            plan.Element.Transform.SetParent(ElementsParent);
                            startedPlans.Add(plan);
                        }
                        else
                        {
                            continue;
                        }

                    IncrementPlan(plan: plan, time: _animationTime);

                    if (_animationTime > plan.EndTime)
                        endedPlans.Add(plan);
                }

                foreach (ElementMovementPlan plan in startedPlans)
                {
                    _elementsMovementPlans[plan.Element] = plan.Copy(isStarted: true);
                    ElementArrangeStarted?.Invoke(sender: this, e: new ArrangementEventArgs(plan));
                }

                foreach (ElementMovementPlan plan in endedPlans)
                {
                    _elementsMovementPlans.Remove(plan.Element);
                    ElementArrangeEnded?.Invoke(sender: this, e: new ArrangementEventArgs(plan));
                }

                startedPlans.Clear();
                endedPlans.Clear();

                yield return null;
                _animationTime += Time.deltaTime;
            }

            _animationTime = 0f;
        }
        
        private static void IncrementPlan(ElementMovementPlan plan, float time)
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
        
        public void RearrangeInstantly()
        {
            this.StopActiveCoroutine();

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
    }
}
