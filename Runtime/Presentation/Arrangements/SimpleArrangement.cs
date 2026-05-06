using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public class SimpleArrangement : Arrangement
    {
        private static readonly DefaultArrangementCalculator _calculator = new();
        
        [field: SerializeField] public Vector2 TargetSize { get; set; }
        [field: SerializeField] public Vector2 TargetSpacing { get; set; }
        
        public override void RecalculateElementPlacements()
        {
            Size = TargetSize;
            
            if (Elements.Count > 1)
            {
                var maxSize = new Vector2(
                    x: Size.x > 0 ? Size.x : float.PositiveInfinity,
                    y: Size.y > 0 ? Size.y : float.PositiveInfinity
                );
                
                Vector2 maxSpacing = maxSize / (Elements.Count - 1);
                Spacing = Vector2.Min(lhs: maxSpacing, rhs: TargetSpacing);
            }
            else
            {
                Spacing = Vector2.zero;
            }

            foreach (ElementPlacement placement in _calculator.CalculateElementPlacements(this))
                SetPlacement(placement);
        }
    }
}
