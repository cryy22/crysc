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
            arrangement.RecalculateElementPlacements();
            IElement[] elementsToAnimate = (elements ?? arrangement.Elements).ToArray();
            if (elementsToAnimate.Length == 0) return arrangement;

            var plans = new ElementMovementPlan[elementsToAnimate.Length];
            for (var i = 0; i < elementsToAnimate.Length; i++)
                plans[i] = CreateMovementPlan(
                    arrangement: arrangement,
                    element: elementsToAnimate[i],
                    startTime: 0f,
                    endTime: duration,
                    extraRotations: extraRotations,
                    easing: easing
                );

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
            arrangement.RecalculateElementPlacements();
            IElement[] elementsAry = (elements ?? arrangement.Elements).ToArray();
            if (elementsAry.Length == 0) return arrangement;

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
            arrangement.RecalculateElementPlacements();
            IElement[] elementsAry = (elements ?? arrangement.Elements).ToArray();
            if (elementsAry.Length == 0) return arrangement;

            var plans = new ElementMovementPlan[elementsAry.Length];
            for (var i = 0; i < elementsAry.Length; i++)
                plans[i] = CreateMovementPlan(
                    arrangement: arrangement,
                    element: elementsAry[i],
                    startTime: 0f,
                    endTime: elementDuration,
                    extraRotations: extraRotations,
                    easing: easing
                );

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
    }
}
