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
        [field: SerializeField] public Vector2 OddElementStagger { get; set; }
        [field: SerializeField] public Vector2 TargetSize { get; set; }
        [field: SerializeField] public Vector2 TargetSpacing { get; set; }

        public int ElementsCount
        {
            get
            {
                var count = 0;
                foreach (SimpleArrangement row in _rows)
                    count += row.Elements.Count;

                return count;
            }
        }

        public IReadOnlyList<SimpleArrangement> Rows => _rows;
        private readonly List<SimpleArrangement> _rows = new();

        public void SetHorizontalAlignment(Arrangement.HorizontalAlignmentType horizontalAlignment)
        {
            foreach (SimpleArrangement row in _rows)
                row.HorizontalAlignment = horizontalAlignment;
        }

        public void SetInverted(bool isInverted)
        {
            foreach (SimpleArrangement row in _rows)
                row.IsInverted = isInverted;
        }

        public void SetElements(IEnumerable<IElement> elements)
        {
            // ensure enough rows
            // populate each row
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

        public void SetMovementPlan(ElementMovementPlan movementPlan, bool relativeTiming = true)
        {
            foreach (SimpleArrangement row in _rows)
                if (row.Elements.Contains(movementPlan.Element))
                {
                    row.SetMovementPlan(plan: movementPlan, relativeTiming: relativeTiming);
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
    }
}
