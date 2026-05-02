using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public class SimpleArrangement : Arrangement
    {
        private static DefaultArrangementCalculator _calculator = new DefaultArrangementCalculator();
        
        [field: SerializeField] public Vector2 MaxSize { get; private set; }
        [field: SerializeField] public Vector2 TargetSpacing { get; private set; }
        
        public override void RecalculateElementPlacements()
        {
            if (_elements.Count > 1)
            {
                var maxSize = new Vector2(
                    x: MaxSize.x > 0 ? MaxSize.x : float.PositiveInfinity,
                    y: MaxSize.y > 0 ? MaxSize.y : float.PositiveInfinity
                );
                Vector2 maxSpacing = maxSize / (_elements.Count - 1);
                Spacing = Vector2.Min(lhs: maxSpacing, rhs: TargetSpacing);

                foreach (ElementPlacement placement in _calculator.CalculateElementPlacements(this))
                    _elementsPlacements[placement.Element] = placement;
            }
        }
    }
}
