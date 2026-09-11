#region

using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public readonly struct SortingSet
    {
        public float ZValue { get; }
        public int SortingLayerId { get; }
        public int SortingOrder { get; }

        public SortingSet(float zValue, int sortingLayerId, int sortingOrder)
        {
            ZValue = zValue;
            SortingLayerId = sortingLayerId;
            SortingOrder = sortingOrder;
        }

        public SortingSet(GameObject go)
        {
            ZValue = go.transform.position.z;
            (SortingLayerId, SortingOrder, _) = go.GetSortingDetails();
        }

        public void SetOnGO(GameObject go)
        {
            go.SetSortingDetails(sortingLayerId: SortingLayerId, sortingOrder: SortingOrder);

            Vector3 position = go.transform.position;
            go.transform.position = new Vector3(x: position.x, y: position.y, z: ZValue);
        }

        public SortingSet Copy(float? zValue = null, int? sortingLayerId = null, int? sortingOrder = null)
        {
            return new SortingSet(
                zValue: zValue ?? ZValue,
                sortingLayerId: sortingLayerId ?? SortingLayerId,
                sortingOrder: sortingOrder ?? SortingOrder
            );
        }
    }
}
