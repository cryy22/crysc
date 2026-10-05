#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common.CoroutineControl;
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
        [SerializeField] private SimpleArrangement RowPrefab;

        [field: SerializeField] public int ElementsPerRow { get; set; }
        [field: SerializeField] public Vector2 TargetSize { get; private set; }
        [field: SerializeField] public Vector2 TargetSpacing { get; private set; }
        [field: SerializeField] public Vector2 OddElementStagger { get; private set; }
        [field: SerializeField] public Arrangement.HorizontalAlignmentType HorizontalAlignment { get; private set; }
        [field: SerializeField] public Arrangement.VerticalAlignmentType VerticalAlignment { get; private set; }
        [field: SerializeField] public bool IsInverted { get; private set; }

        public IReadOnlyList<SimpleArrangement> Rows => _rows;
        private readonly List<SimpleArrangement> _rows = new();

        public int GetElementsCount()
        {
            var count = 0;
            foreach (SimpleArrangement row in _rows)
                count += row.Elements.Count;

            return count;
        }

        public void SetTargetSize(Vector2 targetSize)
        {
            TargetSize = targetSize;
            foreach (SimpleArrangement row in _rows)
                row.TargetSize = targetSize;
        }

        public void SetTargetSpacing(Vector2 targetSpacing)
        {
            TargetSpacing = targetSpacing;
            foreach (SimpleArrangement row in _rows)
                row.TargetSpacing = targetSpacing;
        }

        public void SetOddElementStagger(Vector2 oddElementStagger)
        {
            OddElementStagger = oddElementStagger;
            foreach (SimpleArrangement row in _rows)
                row.OddElementStagger = oddElementStagger;
        }

        public void SetHorizontalAlignment(Arrangement.HorizontalAlignmentType horizontalAlignment)
        {
            HorizontalAlignment = horizontalAlignment;
            foreach (SimpleArrangement row in _rows)
                row.HorizontalAlignment = horizontalAlignment;
        }

        public void SetVerticalAlignment(Arrangement.VerticalAlignmentType verticalAlignment)
        {
            VerticalAlignment = verticalAlignment;
            foreach (SimpleArrangement row in _rows)
                row.VerticalAlignment = verticalAlignment;
        }

        public void SetIsInverted(bool isInverted)
        {
            IsInverted = isInverted;
            foreach (SimpleArrangement row in _rows)
                row.IsInverted = isInverted;
        }

        public void SetElements(IEnumerable<IElement> elements)
        {
            ReadOnlySpan<IElement> elementsSpan = elements.ToArray().AsSpan();
            int targetRowCount = elementsSpan.Length / ElementsPerRow;
            if ((elementsSpan.Length % ElementsPerRow) > 0)
                targetRowCount++;

            while (_rows.Count < targetRowCount)
                InstantiateRow();

            RowsArrangement.SetElements(Rows);
            RowsArrangement.RearrangeInstantly();

            for (var rowIndex = 0; rowIndex < targetRowCount; rowIndex++)
            {
                SimpleArrangement row = Rows[rowIndex];
                int start = rowIndex * ElementsPerRow;
                int end = Mathf.Min(a: start + ElementsPerRow, b: elementsSpan.Length);

                row.SetElements(elementsSpan[start..end]);
            }

            for (int rowIndex = targetRowCount; rowIndex < Rows.Count; rowIndex++)
                Rows[rowIndex].SetElements(Array.Empty<IElement>());
        }

        public IElement GetElementAtIndex(int index)
        {
            foreach (SimpleArrangement row in _rows)
            {
                if (index < row.Elements.Count)
                    return row.Elements[index];

                index -= row.Elements.Count;
            }

            return null;
        }

        public ElementMovementPlan GetMovementPlanForElement(IElement element)
        {
            foreach (SimpleArrangement row in _rows)
                if (row.ElementsMovementPlans.TryGetValue(key: element, value: out ElementMovementPlan movementPlan))
                    return movementPlan;

            throw new ArgumentException($"no element {element} found in ArrangementTable {this}");
        }

        public void RemoveMovementPlanForElement(IElement element)
        {
            foreach (SimpleArrangement row in _rows)
                row.RemoveMovementPlanForElement(element);
        }

        public void SetMovementPlan(ElementMovementPlan plan, bool relativeTiming = true)
        {
            foreach (SimpleArrangement row in _rows)
                if (row.Elements.Contains(plan.Element))
                {
                    row.SetMovementPlan(plan: plan, relativeTiming: relativeTiming);
                    break;
                }
        }

        public ElementPlacement GetPlacementForElement(IElement element)
        {
            foreach (SimpleArrangement row in _rows)
                if (row.ElementsPlacements.TryGetValue(key: element, value: out ElementPlacement placement))
                    return placement;

            throw new ArgumentException($"no element {element} found in ArrangementTable {this}");
        }

        public void RecalculateElementPlacements()
        {
            foreach (SimpleArrangement row in _rows)
                row.RecalculateElementPlacements();
        }

        public void RearrangeInstantly()
        {
            foreach (SimpleArrangement row in _rows)
                row.RearrangeInstantly();
            RowsArrangement.RearrangeInstantly();
        }

        public IEnumerator ExecuteMovementPlansAndWait()
        {
            foreach (SimpleArrangement row in _rows)
                row.ExecuteMovementPlans();

            foreach (SimpleArrangement row in _rows)
                yield return row.WaitForCompletion();
        }

        public void ReparentElements()
        {
            foreach (SimpleArrangement row in _rows)
                row.ReparentElements();
        }

        private void InstantiateRow()
        {
            SimpleArrangement row = Instantiate(RowPrefab);
            row.TargetSize = TargetSize;
            row.TargetSpacing = TargetSpacing;
            row.OddElementStagger = OddElementStagger;
            row.HorizontalAlignment = HorizontalAlignment;
            row.VerticalAlignment = VerticalAlignment;

            _rows.Add(row);
        }
    }
}
