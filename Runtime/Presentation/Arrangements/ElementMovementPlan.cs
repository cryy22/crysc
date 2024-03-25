using JetBrains.Annotations;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public struct ElementMovementPlan
    {
        public IArrangementElement Element { get; }
        public float BeginTime { get; }
        public float EndTime { get; }
        public Vector3 StartPosition { get; }
        public Quaternion StartRotation { get; }
        public Vector3 StartScale { get; }
        public bool IsBegun { get; }
        public bool IsCompleted { get; }

        public float Duration => EndTime - BeginTime;

        public ElementMovementPlan(
            IArrangementElement element,
            float beginTime,
            float endTime,
            Vector3 startPosition,
            Quaternion startRotation,
            Vector3 startScale,
            bool isBegun = false,
            bool isCompleted = false
        )
        {
            Element = element;
            BeginTime = beginTime;
            EndTime = endTime;
            StartPosition = startPosition;
            StartRotation = startRotation;
            StartScale = startScale;
            IsBegun = isBegun;
            IsCompleted = isCompleted;
        }

        public ElementMovementPlan Copy(
            [CanBeNull] IArrangementElement element = null,
            float? beginTime = null,
            float? endTime = null,
            Vector3? startPosition = null,
            Quaternion? startRotation = null,
            Vector3? startScale = null,
            bool? isBegun = null,
            bool? isCompleted = null
        )
        {
            return new ElementMovementPlan(
                element: element ?? Element,
                beginTime: beginTime ?? BeginTime,
                endTime: endTime ?? EndTime,
                startPosition: startPosition ?? StartPosition,
                startRotation: startRotation ?? StartRotation,
                startScale: startScale ?? StartScale,
                isBegun: isBegun ?? IsBegun,
                isCompleted: isCompleted ?? IsCompleted
            );
        }
    }
}
