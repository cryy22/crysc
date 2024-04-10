using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;
    using Plan = ElementMovementPlan;

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
            Plan plan = CreateMovementPlan(
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
            Plan[] plans = GenerateInitialPlansForArrangement(
                arrangement: arrangement,
                elements: elements,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return arrangement;

            ScheduleSimultaneousTiming(plans: plans, duration: duration, consistentSpeed: consistentSpeed);

            foreach (Plan plan in plans) arrangement.SetMovementPlan(plan);
            return arrangement;
        }

        public static void ScheduleSimultaneousMovement(
            IEnumerable<Arrangement> arrangements,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            int extraRotations = 0,
            bool consistentSpeed = true,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            (Plan[] plans, Arrangement[] arrangementsForPlans) = GenerateInitialPlansForArrangements(
                arrangements: arrangements,
                elements: elements,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return;

            ScheduleSimultaneousTiming(plans: plans, duration: duration, consistentSpeed: consistentSpeed);

            for (var i = 0; i < plans.Length; i++)
                arrangementsForPlans[i].SetMovementPlan(plans[i]);
        }

        public static void ScheduleSimultaneousTiming(
            Plan[] plans,
            float duration,
            bool consistentSpeed
        )
        {
            if (consistentSpeed)
            {
                float maxDistance = Mathf.Max(a: plans.Max(p => p.Distance), b: Mathf.Epsilon);
                for (var i = 0; i < plans.Length; i++)
                    plans[i] = plans[i].Copy(endTime: duration * (plans[i].Distance / maxDistance));
            }
            else
            {
                for (var i = 0; i < plans.Length; i++)
                    plans[i] = plans[i].Copy(endTime: duration);
            }
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
            Plan[] plans = GenerateInitialPlansForArrangement(
                arrangement: arrangement,
                elements: elements,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return arrangement;

            ScheduleSerialTiming(
                plans: plans,
                duration: duration,
                spacingPct: spacingPct,
                consistentSpeed: consistentSpeed
            );

            foreach (Plan plan in plans) arrangement.SetMovementPlan(plan);
            return arrangement;
        }

        public static void ScheduleSerialMovement(
            IEnumerable<Arrangement> arrangements,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            float spacingPct = 0f,
            int extraRotations = 0,
            bool consistentSpeed = true,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            (Plan[] plans, Arrangement[] arrangementsForPlans) = GenerateInitialPlansForArrangements(
                arrangements: arrangements,
                elements: elements,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return;

            ScheduleSerialTiming(
                plans: plans,
                duration: duration,
                spacingPct: spacingPct,
                consistentSpeed: consistentSpeed
            );

            for (var i = 0; i < plans.Length; i++)
                arrangementsForPlans[i].SetMovementPlan(plans[i]);
        }

        public static void ScheduleSerialTiming(
            Plan[] plans,
            float duration,
            float spacingPct,
            bool consistentSpeed
        )
        {
            if (plans.Length == 0) return;

            if (consistentSpeed)
            {
                float spacingAwareDistance = plans[0].Distance;

                for (var i = 1; i < plans.Length; i++)
                    spacingAwareDistance += plans[i].Distance * (1 + spacingPct);
                spacingAwareDistance = Mathf.Max(a: spacingAwareDistance, b: Mathf.Epsilon);

                var startTime = 0f;
                var endTime = 0f;
                for (var i = 0; i < plans.Length; i++)
                {
                    Plan plan = plans[i];
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

                for (var i = 0; i < plans.Length; i++)
                {
                    Plan plan = plans[i];
                    plans[i] = plan.Copy(startTime: startTime, endTime: startTime + pDuration);

                    startTime += Mathf.Max(a: (1 + spacingPct) * pDuration, b: 0);
                }
            }
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
            Plan[] plans = GenerateInitialPlansForArrangement(
                arrangement: arrangement,
                elements: elements,
                extraRotations: extraRotations,
                easing: easing
            );
            if (plans.Length == 0) return arrangement;

            var startTime = 0f;
            float delay = initialDelay;
            for (var i = 1; i < plans.Length; i++)
            {
                Plan plan = plans[i];
                startTime += delay;

                plans[i] = plan.Copy(
                    startTime: startTime,
                    endTime: startTime + elementDuration
                );

                delay *= 1 - delayReductionRate;
            }

            foreach (Plan plan in plans) arrangement.SetMovementPlan(plan);
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
                arrangement.ElementsMovementPlans.TryGetValue(key: element, value: out Plan plan);
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
            Plan[] plans = arrangement.ElementsMovementPlans.Values.ToArray();
            EaseMovementPlanTimings(plans: plans, easing: easing);
            foreach (Plan plan in plans)
                arrangement.SetMovementPlan(plan);

            return arrangement;
        }

        public static void EaseMovementPlanTimings(IEnumerable<Arrangement> arrangements, Easings.Enum easing)
        {
            int plansCount = arrangements.SelectMany(a => a.ElementsMovementPlans).Count();
            if (plansCount == 0) return;
            var plans = new Plan[plansCount];
            var arrangementsForPlans = new Arrangement[plansCount];

            var index = 0;
            foreach (Arrangement arrangement in arrangements)
            foreach (Plan plan in arrangement.ElementsMovementPlans.Values)
            {
                plans[index] = plan;
                arrangementsForPlans[index] = arrangement;
                index++;
            }

            EaseMovementPlanTimings(plans: plans, easing: easing);

            for (var i = 0; i < plansCount; i++)
                arrangementsForPlans[i].SetMovementPlan(plans[i]);
        }

        public static void EaseMovementPlanTimings(Plan[] plans, Easings.Enum easing)
        {
            if (plans.Length == 0) return;
            float startDuration = plans.Max(p => p.StartTime);

            for (var i = 0; i < plans.Length; i++)
            {
                Plan plan = plans[i];
                float startTime = Easings.Unease(x: plan.StartTime / startDuration, easing: easing) * startDuration;
                plans[i] = plan.Copy(startTime: startTime, endTime: startTime + plan.Duration);
            }
        }

        public static Plan CreateMovementPlan(
            Arrangement arrangement,
            IElement element,
            float startTime,
            float endTime,
            int extraRotations = 0,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            return new Plan(
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

        public static Plan CreateNoopMovementPlan(IElement element)
        {
            Transform transform = element.Transform;
            return new Plan(
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

        private static Plan[] GenerateInitialPlansForArrangement(
            Arrangement arrangement,
            IEnumerable<IElement> elements,
            int extraRotations,
            Easings.Enum easing
        )
        {
            arrangement.RecalculateElementPlacements();
            IElement[] elementsAry = (elements ?? arrangement.Elements).ToArray();

            var plans = new Plan[elementsAry.Length];
            for (var i = 0; i < elementsAry.Length; i++)
                plans[i] = CreateMovementPlan(
                    arrangement: arrangement,
                    element: elementsAry[i],
                    startTime: 0f,
                    endTime: 0f,
                    extraRotations: extraRotations,
                    easing: easing
                );

            return plans;
        }

        private static (Plan[] plans, Arrangement[] arrangements) GenerateInitialPlansForArrangements(
            IEnumerable<Arrangement> arrangements,
            IEnumerable<IElement> elements,
            int extraRotations,
            Easings.Enum easing
        )
        {
            foreach (Arrangement arrangement in arrangements) arrangement.RecalculateElementPlacements();

            IElement[] elementsAry = (elements ?? arrangements.SelectMany(a => a.Elements)).ToArray();

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

            var plans = new Plan[elementsAry.Length];
            var arrangementsForPlans = new Arrangement[elementsAry.Length];
            for (var i = 0; i < elementsAry.Length; i++)
            {
                IElement element = elementsAry[i];
                elementsArrangements.TryGetValue(key: element, value: out Arrangement arrangement);

                plans[i] = CreateMovementPlan(
                    arrangement: arrangement,
                    element: element,
                    startTime: 0f,
                    endTime: 0f,
                    extraRotations: extraRotations,
                    easing: easing
                );
                arrangementsForPlans[i] = arrangement;
            }

            return (plans, arrangementsForPlans);
        }
    }
}
