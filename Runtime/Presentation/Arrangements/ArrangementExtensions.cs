#region

using System.Linq;
using UnityEngine;

#endregion

namespace Crysc.Presentation.Arrangements
{
    public static class ArrangementExtensions
    {
        public static int GetClosestIndex(this ArrangementTable arrangementTable, Vector2 position)
        {
            var indexOffset = 0;
            int closestIndex = -1;
            var closestDistance = float.MaxValue;

            foreach (SimpleArrangement row in arrangementTable.Arrangements)
            {
                (int index, float distance) = row.GetClosestIndex(position: position, isLocal: false);
                if (distance < closestDistance)
                {
                    closestIndex = index + indexOffset;
                    closestDistance = distance;
                }

                indexOffset += row.Elements.Count;
            }

            return closestIndex;
        }

        public static (int, float) GetClosestIndex(this Arrangement arrangement, Vector2 position, bool isLocal = true)
        {
            int closestIndex = -1;
            var closestDistance = float.MaxValue;

            Vector2 localPosition = isLocal ? position : arrangement.transform.InverseTransformPoint(position);

            for (var i = 0; i < arrangement.Elements.Count; i++)
            {
                IArrangementElement element = arrangement.Elements[i];
                if (!arrangement.ElementsPlacements.TryGetValue(key: element, value: out ElementPlacement placement))
                    continue;

                float distance = Vector2.Distance(a: localPosition, b: placement.Position);

                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }

            return (closestIndex, closestDistance);
        }

        public static int GetInsertionIndex(
            this Arrangement arrangement,
            Vector2 position,
            bool isLocal = true,
            bool useXAxis = true
        )
        {
            Vector2 localPosition = isLocal ? position : arrangement.transform.InverseTransformPoint(position);

            (int closestIndex, _) = arrangement.GetClosestIndex(position: localPosition);
            IArrangementElement closestElement = arrangement.Elements.ElementAtOrDefault(closestIndex);

            if (
                (closestElement == null)
                || !arrangement.ElementsPlacements.TryGetValue(
                    key: closestElement,
                    value: out ElementPlacement placement
                )
            )
                return closestIndex;

            float closestAxialPosition =
                (useXAxis ? placement.Position.x : placement.Position.y) * (arrangement.IsInverted ? -1 : 1);
            float axialPosition = (useXAxis ? localPosition.x : localPosition.y) * (arrangement.IsInverted ? -1 : 1);

            return axialPosition < closestAxialPosition ? closestIndex : closestIndex + 1;
        }
    }
}
