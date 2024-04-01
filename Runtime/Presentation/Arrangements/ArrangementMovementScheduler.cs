using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public static class ArrangementMovementScheduler
    {
        public static void ScheduleElementMovement(
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
        }

        public static void ScheduleSimultaneousMovement(
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
            if (elementsToAnimate.Length == 0) return;

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
        }

        public static void ScheduleSerialMovement(
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
            if (elementsAry.Length == 0) return;

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
                    float pDuration = (plan.Distance / spacingAwareDistance) * duration;
                    startTime = Mathf.Max(a: endTime + (spacingPct * pDuration), b: startTime);
                    endTime = startTime + pDuration;

                    plans[i] = plan.Copy(startTime: startTime, endTime: endTime);
                }
            }
            else
            {
                float pDuration = duration / (1 + ((plans.Length - 1) * (1 + spacingPct)));
                var startTime = 0f;
                var endTime = 0f;
                for (var i = 0; i < plans.Length; i++)
                {
                    ElementMovementPlan plan = plans[i];
                    startTime = Mathf.Max(a: endTime + (spacingPct * pDuration), b: startTime);
                    endTime = startTime + pDuration;

                    plans[i] = plan.Copy(startTime: startTime, endTime: endTime);
                }
            }

            foreach (ElementMovementPlan plan in plans) arrangement.SetMovementPlan(plan);
        }

        public static ElementMovementPlan CreateMovementPlan(
            Arrangement arrangement,
            IElement element,
            float startTime,
            float endTime,
            int extraRotations,
            Easings.Enum easing
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
    }
}
