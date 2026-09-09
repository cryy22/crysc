#region

using UnityEngine;

#endregion

namespace Crysc.Presentation.Arrangements
{
    public class SimpleArrangement : Arrangement
    {
        private static readonly DefaultArrangementCalculator _calculator = new();

        [field: SerializeField] public Vector2 TargetSize { get; set; }
        [field: SerializeField] public Vector2 TargetSpacing { get; set; }

        public override void RecalculateElementPlacements()
        {
            if (Elements.Count > 1)
            {
                if (Mathf.Approximately(a: TargetSize.y, b: -0.05f))
                    Debug.Log("here...");

                var maxSize = new Vector2(
                    x: TargetSize.x > 0 ? TargetSize.x : float.PositiveInfinity,
                    y: TargetSize.y > 0 ? TargetSize.y : float.PositiveInfinity
                );

                Vector2 maxSpacing = maxSize / (Elements.Count - 1);
                Spacing = Vector2.Min(lhs: maxSpacing, rhs: TargetSpacing);

                Vector2 finalElementRelativePosition = CalculateRelativePosition(Elements.Count - 1);
                Vector2 penultimateElementRelativePosition = CalculateRelativePosition(Elements.Count - 2);

                Size = Vector2.Max(
                    lhs: finalElementRelativePosition,
                    rhs: penultimateElementRelativePosition
                );
            }
            else
            {
                Spacing = Vector2.zero;
                Size = Vector2.zero;
            }

            foreach (ElementPlacement placement in _calculator.CalculateElementPlacements(this))
                SetPlacement(placement);
        }

        private Vector2 CalculateRelativePosition(int index)
        {
            return index * Spacing + index % 2 * OddElementStagger;
        }
    }
}
