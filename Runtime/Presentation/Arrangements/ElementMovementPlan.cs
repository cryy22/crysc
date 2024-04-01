using JetBrains.Annotations;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public readonly struct ElementMovementPlan
    {
        public IArrangementElement Element { get; }
        public float StartTime { get; }
        public float EndTime { get; }
        public Vector3 StartPosition { get; }
        public Vector3 EndPosition { get; }
        public Quaternion StartRotation { get; }
        public Quaternion EndRotation { get; }
        public Vector3 StartScale { get; }
        public Vector3 EndScale { get; }
        public int ExtraRotations { get; }
        public Easings.Enum Easing { get; }
        public bool IsStarted { get; }
        public bool IsEnded { get; }

        public float Duration => Mathf.Max(a: EndTime - StartTime, b: Mathf.Epsilon);
        public float Distance => Vector2.Distance(a: StartPosition, b: EndPosition);

        public ElementMovementPlan(
            IArrangementElement element,
            float startTime,
            float endTime,
            Vector3 startPosition,
            Vector3 endPosition,
            Quaternion startRotation,
            Quaternion endRotation,
            Vector3 startScale,
            Vector3 endScale,
            int extraRotations = 0,
            Easings.Enum easing = Easings.Enum.Linear,
            bool isStarted = false,
            bool isEnded = false
        )
        {
            Element = element;
            StartTime = startTime;
            EndTime = endTime;
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartRotation = startRotation;
            EndRotation = endRotation;
            StartScale = startScale;
            EndScale = endScale;
            ExtraRotations = extraRotations;
            Easing = easing;
            IsStarted = isStarted;
            IsEnded = isEnded;

            if (StartTime > EndTime)
                Debug.LogWarning($"invalid ElementMovementTime: start time {StartTime} later than end time {EndTime}");
        }

        public ElementMovementPlan Copy(
            [CanBeNull] IArrangementElement element = null,
            float? startTime = null,
            float? endTime = null,
            Vector3? startPosition = null,
            Vector3? endPosition = null,
            Quaternion? startRotation = null,
            Quaternion? endRotation = null,
            Vector3? startScale = null,
            Vector3? endScale = null,
            int? extraRotations = null,
            bool? isStarted = null,
            bool? isEnded = null,
            Easings.Enum? easing = null
        )
        {
            return new ElementMovementPlan(
                element: element ?? Element,
                startTime: startTime ?? StartTime,
                endTime: endTime ?? EndTime,
                startPosition: startPosition ?? StartPosition,
                endPosition: endPosition ?? EndPosition,
                startRotation: startRotation ?? StartRotation,
                endRotation: endRotation ?? EndRotation,
                startScale: startScale ?? StartScale,
                endScale: endScale ?? EndScale,
                extraRotations: extraRotations ?? ExtraRotations,
                easing: easing ?? Easing,
                isStarted: isStarted ?? IsStarted,
                isEnded: isEnded ?? IsEnded
            );
        }
    }
}
