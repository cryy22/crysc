using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public class SplineArrangementCalculator : MonoBehaviour
    {
        [SerializeField] private SplineContainer SplineContainer;

        private Spline Spline => SplineContainer.Spline;

        // NB, several limitations:
        // - ignores element SizeMultiplier; all elements have the same size
        // - assumes anchor is in the center of the element
        public ElementPlacement[] CalculateElementPlacements(
            IEnumerable<IElement> elements,
            float elementWidth = 1f,
            float preferredSpacingRatio = 0f
        )
        {
            elements = elements.ToArray();
            float splineLength = SplineContainer.CalculateLength();

            float perUnitDistance = GetSplineDistanceBetweenElements(
                elements: elements,
                elementWidth: elementWidth,
                preferredSpacingRatio: preferredSpacingRatio,
                splineLength: splineLength
            );
            float totalDistance = perUnitDistance * (elements.Count() - 1);
            float currentRatio = Mathf.Clamp01(1 - totalDistance) / 2;

            var placements = new ElementPlacement[elements.Count()];
            for (var i = 0; i < elements.Count(); i++)
            {
                Spline.Evaluate(
                    t: currentRatio,
                    position: out float3 position,
                    tangent: out float3 tangent,
                    upVector: out _
                );

                placements[i] = new ElementPlacement(
                    element: elements.ElementAt(i),
                    position: new Vector3(x: position.x, y: position.y, z: Arrangement.ZOffset * i),
                    rotation: Quaternion.FromToRotation(fromDirection: Vector3.right, toDirection: tangent)
                );

                currentRatio += perUnitDistance;
            }

            return placements;
        }

        private float GetSplineDistanceBetweenElements(
            IEnumerable<IElement> elements,
            float elementWidth,
            float preferredSpacingRatio,
            float splineLength
        )
        {
            elements = elements.ToArray();
            if (elements.Count() <= 1) return 0;

            float maxElementSplineRatioWidth = 1f / (elements.Count() - 1);

            float elementSplineRatioWidth = elementWidth / splineLength;
            float preferredElementSplineRatioWidth = elementSplineRatioWidth * (1 + preferredSpacingRatio);

            return Mathf.Min(a: maxElementSplineRatioWidth, b: preferredElementSplineRatioWidth);
        }
    }
}
