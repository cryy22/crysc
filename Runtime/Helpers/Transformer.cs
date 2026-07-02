using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class Transformer
    {
        public static IEnumerator TransformTo(
            Transform transform,
            Vector3 targetPosition,
            Vector3 targetRotation,
            Vector3 targetScale,
            float duration,
            bool isLocal = true
        )
        {
            Vector3 initialPosition = Mover.GetPosition(transform, isLocal);
            Vector3 initialRotation = Rotator.GetRotation(transform, isLocal);
            Vector3 initialScale = transform.localScale;

            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                Mover.MoveToStep(transform: transform, initialPosition, targetPosition, t, isLocal);
                Rotator.SetRotation(transform: transform, Vector3.Lerp(initialRotation, targetRotation, t), isLocal);
                Scaler.ScaleToStep(transform: transform, initialScale, targetScale, t);
                yield return null;
            }

            Mover.SetPosition(transform: transform, targetPosition, isLocal);
            Rotator.SetRotation(transform: transform, targetRotation, isLocal);
            transform.localScale = targetScale;
        }
    }
}
