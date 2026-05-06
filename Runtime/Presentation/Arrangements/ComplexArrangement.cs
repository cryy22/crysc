using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public class ComplexArrangement : Arrangement, IElement
    {
        [FormerlySerializedAs("ElementsParent")]
        [SerializeField] private Transform ElementsParentInput;
        
        [field: FormerlySerializedAs("BaseElementSizeInput")]
        [field: FormerlySerializedAs("BaseElementSize")]
        [field: SerializeField] public Vector2 BaseElementSize { get; set; } = Vector2.right; // prob won't work with a negative
        [field: SerializeField] public Vector2 ElementScale { get; set; } = Vector2.one;
        public override Vector2 ElementSize => BaseElementSize * ElementScale;

        [field: SerializeField] public Vector2 MaxSize { get; set; } = Vector2.zero;
        [field: SerializeField] public Vector2 PreferredSpacingRatio { get; set; } = Vector2.zero;

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 Pivot { get; private set; } = new(x: 0.5f, y: 0.5f);
        public Vector2 SizeMultiplier { get; private set; } = Vector2.zero;

        private bool _isArrangementCalculatorSet;
        private IArrangementCalculator _arrangementCalculator;

        private void Awake()
        {
            if ((BaseElementSize.x < 0) || (BaseElementSize.y < 0))
                throw new Exception("BaseElementSize cannot be negative");
        }

        public override void RecalculateElementPlacements()
        {
            UpdateProperties();
            ElementPlacement[] elementPlacements = GetArrangementCalculator().CalculateElementPlacements(this);
            
            foreach (ElementPlacement placement in elementPlacements)
                SetPlacement(placement.Copy(scale: (Vector3) ElementScale + Vector3.forward));
        }
        
        public void UpdateProperties()
        {
            Vector2 totalSize = Elements.Aggregate(
                seed: Vector2.zero,
                (acc, e) => acc + e.SizeMultiplier
            ) * ElementSize;

            if (Elements.Count > 1)
            {
                var maxSize = new Vector2(
                    x: MaxSize.x > 0 ? MaxSize.x : float.PositiveInfinity,
                    y: MaxSize.y > 0 ? MaxSize.y : float.PositiveInfinity
                );
                Vector2 maxSpacing = (maxSize - totalSize) / (Elements.Count - 1);
                Vector2 preferredSpacing = PreferredSpacingRatio * ElementSize;

                Spacing = Vector2.Min(lhs: maxSpacing, rhs: preferredSpacing);
            }
            else
            {
                Spacing = Vector2.zero;
            }

            Size = totalSize + Spacing * (Elements.Count - 1);

            SizeMultiplier = new Vector2(
                x: ElementSize.x > 0 ? Size.x / ElementSize.x : 0,
                y: ElementSize.y > 0 ? Size.y / ElementSize.y : 0
            );

            Pivot = new Vector2(
                x: HorizontalAlignment switch
                {
                    HorizontalAlignmentType.Left   => IsInverted ? 1 : 0,
                    HorizontalAlignmentType.Center => 0.5f,
                    HorizontalAlignmentType.Right  => IsInverted ? 0 : 1,
                    _                              => throw new ArgumentOutOfRangeException(),
                },
                y: VerticalAlignment switch
                {
                    VerticalAlignmentType.Bottom => 0,
                    VerticalAlignmentType.Middle => 0.5f,
                    VerticalAlignmentType.Top    => 1,
                    _                            => throw new ArgumentOutOfRangeException(),
                }
            );
        }


        private IArrangementCalculator GetArrangementCalculator()
        {
            if (_isArrangementCalculatorSet) return _arrangementCalculator;

            _arrangementCalculator = GetComponent<IArrangementCalculator>();
            if (_arrangementCalculator == null) _arrangementCalculator = new DefaultArrangementCalculator();
            _isArrangementCalculatorSet = true;

            return _arrangementCalculator;
        }
    }
}
