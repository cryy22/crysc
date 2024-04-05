using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public static class ArrangementMovementScheduler
    {
        public static Arrangement ScheduleElementMovement(
            this Arrangement arrangement,
            IElement element,
            float startTime = 0f,
            float duration = 0.25f,
            int extraRotations = 0,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            arrangement.RecalculateElementPlacements();
            ElementMovementPlan plan = CreateMovementPlan(
                arrangement: arrangement,
                element: element,
                startTime: startTime,
                endTime: duration,
                extraRotations: extraRotations,
                easing: easing
            );

            arrangement.SetMovementPlan(plan);
            return arrangement;
        }

        public static Arrangement ScheduleSimultaneousMovement(
            this Arrangement arrangement,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            int extraRotations = 0,
            bool consistentSpeed = true,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            ElementMovementPlan[] plans = GenerateInitialPlansForArrangement(
                arrangement: arrangement,
                elements: elements,
                duration: duration,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return arrangement;

            if (consistentSpeed)
            {
                float maxDistance = Mathf.Max(a: plans.Max(p => p.Distance), b: Mathf.Epsilon);
                for (var i = 0; i < plans.Length; i++)
                    plans[i] = plans[i].Copy(endTime: duration * (plans[i].Distance / maxDistance));
            }

            foreach (ElementMovementPlan plan in plans) arrangement.SetMovementPlan(plan);
            return arrangement;
        }

        public static Arrangement ScheduleSerialMovement(
            this Arrangement arrangement,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            float spacingPct = 0f,
            int extraRotations = 0,
            bool consistentSpeed = true,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            ElementMovementPlan[] plans = GenerateInitialPlansForArrangement(
                arrangement: arrangement,
                elements: elements,
                duration: duration,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return arrangement;

            if (consistentSpeed)
            {
                var spacingAwareDistance = 0f;
                for (var i = 0; i < plans.Length; i++)
                    spacingAwareDistance += plans[i].Distance * (1 + (i > 0 ? spacingPct : 0));
                spacingAwareDistance = Mathf.Max(a: spacingAwareDistance, b: Mathf.Epsilon);

                var startTime = 0f;
                var endTime = 0f;
                for (var i = 0; i < plans.Length; i++)
                {
                    ElementMovementPlan plan = plans[i];
                    float pDuration = plan.Distance / spacingAwareDistance * duration;
                    startTime = Mathf.Max(a: endTime + spacingPct * pDuration, b: startTime);
                    endTime = startTime + pDuration;

                    plans[i] = plan.Copy(startTime: startTime, endTime: endTime);
                }
            }
            else
            {
                float pDuration = duration / (1 + (plans.Length - 1) * (1 + spacingPct));
                var startTime = 0f;
                var endTime = 0f;
                for (var i = 0; i < plans.Length; i++)
                {
                    ElementMovementPlan plan = plans[i];
                    startTime = Mathf.Max(a: endTime + spacingPct * pDuration, b: startTime);
                    endTime = startTime + pDuration;

                    plans[i] = plan.Copy(startTime: startTime, endTime: endTime);
                }
            }

            foreach (ElementMovementPlan plan in plans) arrangement.SetMovementPlan(plan);
            return arrangement;
        }

        public static void ScheduleSerialMovement(
            Arrangement[] arrangements,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            float spacingPct = 0f,
            int extraRotations = 0,
            bool consistentSpeed = true,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            foreach (Arrangement arrangement in arrangements) arrangement.RecalculateElementPlacements();

            IElement[] elementsAry = (elements ?? arrangements.SelectMany(a => a.Elements)).ToArray();
            if (elementsAry.Length == 0) return;

            var elementsArrangements = new Dictionary<IElement, Arrangement>();
            foreach (Arrangement arrangement in arrangements)
            foreach (IElement element in arrangement.Elements)
            {
                if (elementsAry.Contains(element) == false) continue;
                elementsArrangements[element] = arrangement;
            }

            if (elementsAry.Length > elementsArrangements.Count)
                elementsAry = elementsAry
                    .Distinct()
                    .Where(e => elementsArrangements.ContainsKey(e))
                    .ToArray();

            var plans = new ElementMovementPlan[elementsAry.Length];
            for (var i = 0; i < elementsAry.Length; i++)
            {
                IElement element = elementsAry[i];
                elementsArrangements.TryGetValue(key: element, value: out Arrangement arrangement);
                if (!arrangement) continue;

                plans[i] = CreateMovementPlan(
                    arrangement: arrangement,
                    element: element,
                    startTime: 0f,
                    endTime: duration,
                    extraRotations: extraRotations,
                    easing: easing
                );
            }

            if (consistentSpeed)
            {
                var spacingAwareDistance = 0f;
                for (var i = 0; i < plans.Length; i++)
                    spacingAwareDistance += plans[i].Distance * (1 + (i > 0 ? spacingPct : 0));
                spacingAwareDistance = Mathf.Max(a: spacingAwareDistance, b: Mathf.Epsilon);

                var startTime = 0f;
                var endTime = 0f;
                for (var i = 0; i < plans.Length; i++)
                {
                    ElementMovementPlan plan = plans[i];
                    float pDuration = plan.Distance / spacingAwareDistance * duration;
                    startTime = Mathf.Max(a: endTime + spacingPct * pDuration, b: startTime);
                    endTime = startTime + pDuration;

                    plans[i] = plan.Copy(startTime: startTime, endTime: endTime);
                }
            }
            else
            {
                float pDuration = duration / (1 + (plans.Length - 1) * (1 + spacingPct));
                var startTime = 0f;
                var endTime = 0f;
                for (var i = 0; i < plans.Length; i++)
                {
                    ElementMovementPlan plan = plans[i];
                    startTime = Mathf.Max(a: endTime + spacingPct * pDuration, b: startTime);
                    endTime = startTime + pDuration;

                    plans[i] = plan.Copy(startTime: startTime, endTime: endTime);
                }
            }

            foreach (ElementMovementPlan plan in plans)
                elementsArrangements[plan.Element].SetMovementPlan(plan);
        }

        public static Arrangement ScheduleAcceleratingMovement(
            this Arrangement arrangement,
            IEnumerable<IElement> elements = null,
            float elementDuration = 0.25f,
            float initialDelay = 0.5f,
            float delayReductionRate = 0.1f,
            int extraRotations = 0,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            ElementMovementPlan[] plans = GenerateInitialPlansForArrangement(
                arrangement: arrangement,
                elements: elements,
                duration: elementDuration,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return arrangement;

            var startTime = 0f;
            float delay = initialDelay;
            for (var i = 1; i < plans.Length; i++)
            {
                ElementMovementPlan plan = plans[i];
                startTime += delay;

                plans[i] = plan.Copy(
                    startTime: startTime,
                    endTime: plans[i].EndTime + startTime
                );

                delay *= 1 - delayReductionRate;
            }

            foreach (ElementMovementPlan plan in plans) arrangement.SetMovementPlan(plan);
            return arrangement;
        }

        public static Arrangement MussMovementPlans(
            this Arrangement arrangement,
            IEnumerable<IElement> elements = null,
            float mussRadius = 0.75f,
            float mussRotationRange = 45f
        )
        {
            IElement[] elementsAry = (elements ?? arrangement.Elements).ToArray();
            if (elementsAry.Length == 0) return arrangement;

            foreach (IElement element in elementsAry)
            {
                arrangement.ElementsMovementPlans.TryGetValue(key: element, value: out ElementMovementPlan plan);
                plan = plan.Element == null ? CreateNoopMovementPlan(element) : plan;
                arrangement.SetMovementPlan(
                    plan.Copy(
                        endPosition: plan.EndPosition + (Vector3) (Random.insideUnitCircle * mussRadius),
                        endRotation: plan.EndRotation * Quaternion.Euler(
                            x: 0,
                            y: 0,
                            z: Random.Range(
                                minInclusive: -(mussRotationRange / 2),
                                maxInclusive: mussRotationRange / 2
                            )
                        )
                    )
                );
            }

            return arrangement;
        }

        public static Arrangement EaseMovementPlanTimings(this Arrangement arrangement, Easings.Enum easing)
        {
            ElementMovementPlan[] plans = arrangement.ElementsMovementPlans.Values.ToArray();
            float startDuration = plans.Max(p => p.StartTime);

            foreach (ElementMovementPlan plan in plans)
            {
                float startTime = Easings.Ease(t: plan.StartTime / startDuration, easing: easing) * startDuration;
                arrangement.SetMovementPlan(
                    plan.Copy(startTime: startTime, endTime: startTime + plan.Duration)
                );
            }

            return arrangement;
        }

        public static ElementMovementPlan CreateMovementPlan(
            Arrangement arrangement,
            IElement element,
            float startTime,
            float endTime,
            int extraRotations = 0,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            return new ElementMovementPlan(
                element: element,
                startTime: startTime,
                endTime: endTime,
                startPosition: element.Transform.localPosition,
                endPosition: arrangement.ElementsPlacements[element].Position,
                startRotation: element.Transform.localRotation,
                endRotation: arrangement.ElementsPlacements[element].Rotation,
                startScale: element.Transform.localScale,
                endScale: Vector3.one,
                extraRotations: extraRotations,
                easing: easing
            );
        }

        public static ElementMovementPlan CreateNoopMovementPlan(IElement element)
        {
            Transform transform = element.Transform;
            return new ElementMovementPlan(
                element: element,
                startTime: 0f,
                endTime: 0f,
                startPosition: transform.localPosition,
                endPosition: transform.localPosition,
                startRotation: transform.localRotation,
                endRotation: transform.localRotation,
                startScale: transform.localScale,
                endScale: transform.localScale,
                extraRotations: 0,
                easing: Easings.Enum.Linear
            );
        }

        private static ElementMovementPlan[] GenerateInitialPlansForArrangement(
            Arrangement arrangement,
            IEnumerable<IElement> elements,
            float duration,
            int extraRotations,
            Easings.Enum easing
        )
        {
            arrangement.RecalculateElementPlacements();
            IElement[] elementsAry = (elements ?? arrangement.Elements).ToArray();

            var plans = new ElementMovementPlan[elementsAry.Length];
            for (var i = 0; i < elementsAry.Length; i++)
                plans[i] = CreateMovementPlan(
                    arrangement: arrangement,
                    element: elementsAry[i],
                    startTime: 0f,
                    endTime: duration,
                    extraRotations: extraRotations,
                    easing: easing
                );

            return plans;
        }
    }
}
