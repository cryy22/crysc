using System.Linq;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public class DefaultArrangementCalculator : IArrangementCalculator
    {
        public ElementPlacement[] CalculateElementPlacements(Arrangement arrangement)
        {
            IElement[] elementsAry = arrangement.Elements.ToArray();

            var placements = new ElementPlacement[elementsAry.Length];
            Vector2 weightedIndexes = Vector2.zero;

            for (var i = 0; i < elementsAry.Length; i++)
            {
                IElement element = elementsAry[i];

                Vector3 startPoint = CalculateElementStartPoint(
                    arrangement: arrangement,
                    weightedIndexes: weightedIndexes,
                    i: i
                );
                placements[i] = new ElementPlacement(
                    element: element,
                    position: CalculateElementAnchorPoint(
                        arrangement: arrangement,
                        element: element,
                        startPoint: startPoint
                    ),
                    rotation: Quaternion.identity
                );

                weightedIndexes += element.SizeMultiplier;
            }

            return placements;
        }

        private static Vector3 CalculateElementStartPoint(Arrangement arrangement, Vector2 weightedIndexes, int i)
        {
            Vector2 startPoint2d = arrangement.BaseElementSize * weightedIndexes;
            startPoint2d += arrangement.Spacing * i;
            startPoint2d -= arrangement.AlignmentOffset;
            if (i % 2 == 1) startPoint2d += arrangement.OddElementStagger;
            startPoint2d *= arrangement.Direction;

            return new Vector3(
                x: startPoint2d.x,
                y: startPoint2d.y,
                z: Arrangement.ZOffset * i
            );
        }

        private static Vector3 CalculateElementAnchorPoint(
            Arrangement arrangement,
            IElement element,
            Vector3 startPoint
        )
        {
            Vector2 elementSize = arrangement.BaseElementSize * element.SizeMultiplier;
            Vector2 directionalPivot = element.Pivot - (arrangement.IsInverted ? Vector2.one : Vector2.zero);
            Vector2 midpoint2d = (Vector2) startPoint + (elementSize * directionalPivot);

            return new Vector3(x: midpoint2d.x, y: midpoint2d.y, z: startPoint.z) + element.ArrangementOffset;
        }
    }
}
