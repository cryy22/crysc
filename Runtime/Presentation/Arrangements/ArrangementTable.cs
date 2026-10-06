#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common.CoroutineControl;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Crysc.Presentation.Arrangements
{
    #region

    using IElement = IArrangementElement;

    #endregion

    public class ArrangementTable : MonoBehaviour
    {
        [SerializeField] private SimpleArrangement RowsArrangement;
        [SerializeField] private ArrangementTableRow RowPrefab;

        [field: SerializeField] public int ElementsPerRow { get; set; }
        [field: SerializeField] public Vector2 TargetSize { get; private set; }
        [field: SerializeField] public Vector2 TargetSpacing { get; private set; }
        [field: SerializeField] public Vector2 OddElementStagger { get; private set; }
        [field: SerializeField] public Arrangement.HorizontalAlignmentType HorizontalAlignment { get; private set; }
        [field: SerializeField] public Arrangement.VerticalAlignmentType VerticalAlignment { get; private set; }
        [field: SerializeField] public bool IsInverted { get; private set; }

        [field: SerializeField, ValueDropdown("@SortingLayersUtility.GetSortingLayers()")]
        public string SortingLayer { get; private set; }
        [field: SerializeField] public int SortOrder { get; private set; }
        [field: SerializeField] public int SortOrderRowDelta { get; private set; }

        [SerializeField] private List<ParallaxLayerConfig> _ParallaxLayers = new();
        public IReadOnlyList<ParallaxLayerConfig> ParallaxLayers => _ParallaxLayers;
        [field: SerializeField] public bool IsAffectedBySpeed { get; private set; }

        public IReadOnlyList<ArrangementTableRow> Rows => _rows;
        private readonly List<ArrangementTableRow> _rows = new();
        public IReadOnlyList<SimpleArrangement> Arrangements => _arrangements;
        private readonly List<SimpleArrangement> _arrangements = new();

        private readonly HashSet<IElement> _excludedElements = new();

        public int GetElementsCount()
        {
            var count = 0;
            foreach (SimpleArrangement row in Arrangements)
                count += row.Elements.Count;

            return count;
        }

        public void SetTargetSize(Vector2 targetSize)
        {
            TargetSize = targetSize;
            foreach (SimpleArrangement row in Arrangements)
                row.TargetSize = targetSize;
        }

        public void SetTargetSpacing(Vector2 targetSpacing)
        {
            TargetSpacing = targetSpacing;
            foreach (SimpleArrangement row in Arrangements)
                row.TargetSpacing = targetSpacing;
        }

        public void SetOddElementStagger(Vector2 oddElementStagger)
        {
            OddElementStagger = oddElementStagger;
            foreach (SimpleArrangement row in Arrangements)
                row.OddElementStagger = oddElementStagger;
        }

        public void SetHorizontalAlignment(Arrangement.HorizontalAlignmentType horizontalAlignment)
        {
            HorizontalAlignment = horizontalAlignment;
            foreach (SimpleArrangement row in Arrangements)
                row.HorizontalAlignment = horizontalAlignment;
        }

        public void SetVerticalAlignment(Arrangement.VerticalAlignmentType verticalAlignment)
        {
            VerticalAlignment = verticalAlignment;
            foreach (SimpleArrangement row in Arrangements)
                row.VerticalAlignment = verticalAlignment;
        }

        public void SetIsInverted(bool isInverted)
        {
            IsInverted = isInverted;
            foreach (SimpleArrangement row in Arrangements)
                row.IsInverted = isInverted;
        }

        public void SetSortingDetails(string sortingLayer, int sortOrder, int sortOrderRowDelta)
        {
            SortingLayer = sortingLayer;
            SortOrder = sortOrder;
            SortOrderRowDelta = sortOrderRowDelta;

            for (var i = 0; i < _rows.Count; i++)
            {
                ArrangementTableRow row = _rows[i];
                row.SortingEnforcer.SetSortingLayer(SortingLayer);
                row.SortingEnforcer.SetSortOrder(SortOrder + i * SortOrderRowDelta);
            }
        }

        public void SetParallaxLayers(IReadOnlyList<ParallaxLayerConfig> layers, bool isAffectedBySpeed)
        {
            _ParallaxLayers.Clear();
            _ParallaxLayers.AddRange(layers);
            IsAffectedBySpeed = isAffectedBySpeed;

            ParallaxLayerConfig currentConfig = null;
            for (var i = 0; i < _rows.Count; i++)
            {
                if (_ParallaxLayers.Count > i)
                    currentConfig = _ParallaxLayers[i];

                ArrangementTableRow row = _rows[i];
                row.ParallaxRegistrar.Register(layer: currentConfig, isAffectedBySpeed: IsAffectedBySpeed);
            }
        }

        public void SetElements(IEnumerable<IElement> elements)
        {
            ReadOnlySpan<IElement> elementsSpan = elements.ToArray().AsSpan();
            int targetRowCount = elementsSpan.Length / ElementsPerRow;
            if ((elementsSpan.Length % ElementsPerRow) > 0)
                targetRowCount++;

            while (_rows.Count < targetRowCount)
                InstantiateRow();

            RowsArrangement.SetElements(_rows);
            RowsArrangement.RearrangeInstantly();

            for (var rowIndex = 0; rowIndex < targetRowCount; rowIndex++)
            {
                SimpleArrangement row = Arrangements[rowIndex];
                int start = rowIndex * ElementsPerRow;
                int end = Mathf.Min(a: start + ElementsPerRow, b: elementsSpan.Length);

                row.SetElements(elementsSpan[start..end]);
            }

            for (int rowIndex = targetRowCount; rowIndex < _rows.Count; rowIndex++)
                Arrangements[rowIndex].SetElements(Array.Empty<IElement>());
        }

        public IElement GetElementAtIndex(int index)
        {
            foreach (SimpleArrangement row in Arrangements)
            {
                if (index < row.Elements.Count)
                    return row.Elements[index];

                index -= row.Elements.Count;
            }

            return null;
        }

        public ElementMovementPlan GetMovementPlanForElement(IElement element)
        {
            foreach (SimpleArrangement row in Arrangements)
                if (row.ElementsMovementPlans.TryGetValue(key: element, value: out ElementMovementPlan movementPlan))
                    return movementPlan;

            throw new ArgumentException($"no element {element} found in ArrangementTable {this}");
        }

        public void RemoveMovementPlanForElement(IElement element)
        {
            foreach (SimpleArrangement row in Arrangements)
                row.RemoveMovementPlanForElement(element);
        }

        public void SetMovementPlan(ElementMovementPlan plan, bool relativeTiming = true)
        {
            foreach (SimpleArrangement row in Arrangements)
                if (row.Elements.Contains(plan.Element))
                {
                    row.SetMovementPlan(plan: plan, relativeTiming: relativeTiming);
                    break;
                }
        }

        public ElementPlacement GetPlacementForElement(IElement element)
        {
            foreach (SimpleArrangement row in Arrangements)
                if (row.ElementsPlacements.TryGetValue(key: element, value: out ElementPlacement placement))
                    return placement;

            throw new ArgumentException($"no element {element} found in ArrangementTable {this}");
        }

        public void AddToExcludedElements(IElement element)
        {
            _excludedElements.Add(element);
            foreach (SimpleArrangement arrangement in _arrangements)
                arrangement.AddToExcludedElements(element);
        }

        public void RemoveFromExcludedElements(IElement element)
        {
            _excludedElements.Remove(element);
            foreach (SimpleArrangement arrangement in _arrangements)
                arrangement.RemoveFromExcludedElements(element);
        }

        public void RecalculateElementPlacements()
        {
            foreach (SimpleArrangement row in Arrangements)
                row.RecalculateElementPlacements();
        }

        public void RearrangeInstantly()
        {
            foreach (SimpleArrangement row in Arrangements)
                row.RearrangeInstantly();
            RowsArrangement.RearrangeInstantly();
        }

        public void ExecuteMovementPlans()
        {
            foreach (SimpleArrangement row in Arrangements)
                row.ExecuteMovementPlans();
        }

        public IEnumerator ExecuteMovementPlansAndWait()
        {
            ExecuteMovementPlans();
            foreach (SimpleArrangement row in Arrangements)
                yield return row.WaitForCompletion();
        }

        public IEnumerator WaitForCompletion()
        {
            foreach (SimpleArrangement row in Arrangements)
                yield return row.WaitForCompletion();
        }

        public void ReparentElements()
        {
            foreach (SimpleArrangement row in Arrangements)
                row.ReparentElements();
        }

        private void InstantiateRow()
        {
            ArrangementTableRow row = Instantiate(RowPrefab);

            row.Arrangement.TargetSize = TargetSize;
            row.Arrangement.TargetSpacing = TargetSpacing;
            row.Arrangement.OddElementStagger = OddElementStagger;
            row.Arrangement.HorizontalAlignment = HorizontalAlignment;
            row.Arrangement.VerticalAlignment = VerticalAlignment;
            foreach (IElement element in _excludedElements)
                row.Arrangement.AddToExcludedElements(element);

            row.SortingEnforcer.SetSortingLayer(SortingLayer);
            row.SortingEnforcer.SetSortOrder(SortOrder + _rows.Count * SortOrderRowDelta);

            _rows.Add(row);
            _arrangements.Add(row.Arrangement);
        }
    }
}
