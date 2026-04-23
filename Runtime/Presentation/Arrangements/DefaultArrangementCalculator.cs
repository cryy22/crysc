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

                placements[i] = new ElementPlacement(
                    element: element,
                    position: CalculateElementAnchorPoint(
                        arrangement: arrangement,
                        element: element,
                        weightedIndexes: weightedIndexes,
                        i: i
                    ),
                    rotation: Quaternion.identity
                );

                weightedIndexes += element.SizeMultiplier;
            }

            return placements;
        }

        private static Vector3 CalculateElementAnchorPoint(
            Arrangement arrangement,
            IElement element,
            Vector2 weightedIndexes,
            int i
        )
        {
            Vector2 elementSize = arrangement.ElementSize * element.SizeMultiplier;
            Vector2 directionalPivot = element.Pivot - (arrangement.IsInverted ? Vector2.one : Vector2.zero);

            Vector3 startPoint = CalculateElementStartPoint(
                arrangement: arrangement,
                weightedIndexes: weightedIndexes,
                i: i
            );
            Vector2 midpoint2d = (Vector2) startPoint + (elementSize * directionalPivot);

            return new Vector3(x: midpoint2d.x, y: midpoint2d.y, z: startPoint.z) + element.ArrangementOffset;
        }

        private static Vector3 CalculateElementStartPoint(Arrangement arrangement, Vector2 weightedIndexes, int i)
        {
            Vector2 startPoint2D = arrangement.ElementSize * weightedIndexes;
            startPoint2D += arrangement.Spacing * i;
            startPoint2D -= arrangement.AlignmentOffset;
            if (i % 2 == 1) startPoint2D += arrangement.OddElementStagger;
            startPoint2D *= arrangement.Direction;

            return new Vector3(
                x: startPoint2D.x,
                y: startPoint2D.y,
                z: Arrangement.ZOffset * i
            );
        }
    }
}
