using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public static class ArrangementAnimation
    {
        public static IEnumerator AnimateElement(
            this Arrangement arrangement,
            IElement element,
            float duration = 0.25f,
            int extraRotations = 0,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            arrangement.AnimatingElements.Add(element);

            ElementMovementPlan plan = CreateMovementPlan(
                arrangement: arrangement,
                element: element,
                startTime: 0f,
                endTime: duration,
                extraRotations: extraRotations,
                easing: easing
            );

            var time = 0f;
            while (time <= duration)
            {
                time += Time.deltaTime;
                IncrementPlan(plan: plan, time: time);
                yield return null;
            }

            arrangement.AnimatingElements.Remove(element);
        }

        public static IEnumerator AnimateElementsSimultaneously(
            this Arrangement arrangement,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            int extraRotations = 0,
            bool consistentSpeed = true,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            IElement[] elementsAry = (elements ?? arrangement.Elements).ToArray();
            arrangement.AnimatingElements.UnionWith(elementsAry);

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
                float maxDistance = plans.Max(p => p.Distance);
                for (var i = 0; i < plans.Length; i++)
                    plans[i] = plans[i].Copy(endTime: duration * (plans[i].Distance / maxDistance));
            }

            var time = 0f;
            while (time <= duration)
            {
                time += Time.deltaTime;
                foreach (ElementMovementPlan plan in plans)
                    IncrementPlan(plan: plan, time: time);
                yield return null;
            }

            arrangement.AnimatingElements.ExceptWith(elementsAry);
        }

        public static IEnumerator AnimateElementsSerially(
            this Arrangement arrangement,
            IEnumerable<IElement> elements = null,
            float duration = 0.25f,
            float spacingPct = 0f,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            yield break;
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
    }
}
