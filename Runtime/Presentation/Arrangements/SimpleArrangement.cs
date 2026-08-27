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
                var maxSize = new Vector2(
                    x: TargetSize.x > 0 ? TargetSize.x : float.PositiveInfinity,
                    y: TargetSize.y > 0 ? TargetSize.y : float.PositiveInfinity
                );

                Vector2 maxSpacing = maxSize / (Elements.Count - 1);
                Spacing = Vector2.Min(lhs: maxSpacing, rhs: TargetSpacing);

                Size = Spacing * (Elements.Count - 1);
                Size = new Vector2(x: Mathf.Max(a: Size.x, b: 0), y: Mathf.Max(a: Size.y, b: 0));
            }
            else
            {
                Spacing = Vector2.zero;
                Size = Vector2.zero;
            }

            foreach (ElementPlacement placement in _calculator.CalculateElementPlacements(this))
                SetPlacement(placement);
        }
    }
}
