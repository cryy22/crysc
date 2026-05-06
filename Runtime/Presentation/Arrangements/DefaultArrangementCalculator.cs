using System;
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

            var alignmentAdjustment = new Vector2(
                x: arrangement.HorizontalAlignment switch
                {
                    Arrangement.HorizontalAlignmentType.Left => 0f,
                    Arrangement.HorizontalAlignmentType.Center => -arrangement.Size.x / 2f,
                    Arrangement.HorizontalAlignmentType.Right => -arrangement.Size.x,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                y: arrangement.VerticalAlignment switch
                {
                    Arrangement.VerticalAlignmentType.Bottom => 0f,
                    Arrangement.VerticalAlignmentType.Middle => -arrangement.Size.y / 2f,
                    Arrangement.VerticalAlignmentType.Top => -arrangement.Size.y,
                    _ => throw new ArgumentOutOfRangeException(),
                }
            );

            if (arrangement.IsInverted)
                alignmentAdjustment += new Vector2(x: arrangement.Size.x, 0);

            for (var i = 0; i < elementsAry.Length; i++)
            {
                IElement element = elementsAry[i];

                placements[i] = new ElementPlacement(
                    element: element,
                    position: CalculateElementAnchorPoint(
                        arrangement: arrangement,
                        element: element,
                        weightedIndexes: weightedIndexes,
                        alignmentAdjustment: alignmentAdjustment,
                        i: i
                    ),
                    rotation: Quaternion.identity,
                    scale: Vector3.one
                );

                weightedIndexes += element.SizeMultiplier;
            }

            return placements;
        }

        private static Vector3 CalculateElementAnchorPoint(
            Arrangement arrangement,
            IElement element,
            Vector2 weightedIndexes,
            Vector2 alignmentAdjustment,
            int i
        )
        {
            Vector2 elementSize = arrangement.ElementSize * element.SizeMultiplier;
            Vector2 directionalPivot = element.Pivot - (arrangement.IsInverted ? Vector2.one : Vector2.zero);

            Vector3 startPoint = CalculateElementStartPoint(
                arrangement: arrangement,
                weightedIndexes: weightedIndexes,
                alignmentAdjustment: alignmentAdjustment,
                i: i
            );
            Vector2 midpoint2D = (Vector2) startPoint + (elementSize * directionalPivot);
            
            return new Vector3(x: midpoint2D.x, y: midpoint2D.y, z: startPoint.z) + element.ArrangementOffset;
        }

        private static Vector3 CalculateElementStartPoint(
            Arrangement arrangement, 
            Vector2 weightedIndexes, 
            Vector2 alignmentAdjustment,
            int i
        )
        {
            Vector2 startPoint2D = arrangement.ElementSize * weightedIndexes;
            startPoint2D += arrangement.Spacing * i;
            startPoint2D += alignmentAdjustment;
            startPoint2D += arrangement.OddElementStagger * (i % 2);
            startPoint2D *= new Vector2(
                x: arrangement.IsInverted ? -1 : 1,
                y: 1
            );

            return new Vector3(
                x: startPoint2D.x,
                y: startPoint2D.y,
                z: Arrangement.ZOffset * i
            );
        }
    }
}
