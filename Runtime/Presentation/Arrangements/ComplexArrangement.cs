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
        public Vector2 ElementSize => BaseElementSize * ElementScale;
        
        [FormerlySerializedAs("OddElementStaggerInput")]
        [SerializeField] public Vector2 OddElementStagger = Vector2.zero;

        [field: SerializeField] public Alignment HorizontalAlignment { get; set; } = Alignment.Left;
        [field: SerializeField] public bool IsInverted { get; set; }
        [field: SerializeField] public Vector2 MaxSize { get; set; } = Vector2.zero;
        [field: SerializeField] public Vector2 PreferredSpacingRatio { get; set; } = Vector2.zero;

        public Vector2 Direction => Vector2.one * (IsInverted ? -1 : 1);

        public Vector2 AlignmentOffset { get; private set; }
        public Vector2 Spacing { get; private set; } = Vector2.zero;

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

        public void RemoveMovementPlanForElement(IElement element) { _elementsMovementPlans.Remove(element); }

        public void UpdateProperties()
        {
            Vector2 totalSize = _elements.Aggregate(
                seed: Vector2.zero,
                (acc, e) => acc + e.SizeMultiplier
            ) * ElementSize;

            if (_elements.Count > 1)
            {
                var maxSize = new Vector2(
                    x: MaxSize.x > 0 ? MaxSize.x : float.PositiveInfinity,
                    y: MaxSize.y > 0 ? MaxSize.y : float.PositiveInfinity
                );
                Vector2 maxSpacing = (maxSize - totalSize) / (_elements.Count - 1);
                Vector2 preferredSpacing = PreferredSpacingRatio * ElementSize;

                Spacing = Vector2.Min(lhs: maxSpacing, rhs: preferredSpacing);
            }

            Vector2 size = totalSize + Spacing * (_elements.Count - 1);

            SizeMultiplier = new Vector2(
                x: ElementSize.x > 0 ? size.x / ElementSize.x : 0,
                y: ElementSize.y > 0 ? size.y / ElementSize.y : 0
            );
            AlignmentOffset = HorizontalAlignment switch
            {
                Alignment.Left   => Vector2.zero,
                Alignment.Center => size / 2,
                Alignment.Right  => size,
                _                => throw new ArgumentOutOfRangeException(),
            };

            Pivot = HorizontalAlignment switch
            {
                Alignment.Left   => IsInverted ? Vector2.one : Vector2.zero,
                Alignment.Center => new Vector2(x: 0.5f, y: 0.5f),
                Alignment.Right  => IsInverted ? Vector2.zero : Vector2.one,
                _                => throw new ArgumentOutOfRangeException(),
            };
        }

        public override void RecalculateElementPlacements()
        {
            UpdateProperties();
            ElementPlacement[] elementPlacements = GetArrangementCalculator().CalculateElementPlacements(this);
            
            foreach (ElementPlacement placement in elementPlacements)
                _elementsPlacements[placement.Element] = placement.Copy(scale: (Vector3) ElementScale + Vector3.forward);
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
