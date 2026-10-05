#region

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public class SortingEnforcer : MonoBehaviour
    {
        [field: SerializeField, ValueDropdown("@SortingLayersUtility.GetSortingLayers()")]
        public string SortingLayer { get; private set; }
        [field: SerializeField] public int SortOrder { get; private set; }

        private void OnTransformChildrenChanged()
        {
            Refresh();
        }

        public void Refresh()
        {
            foreach (Transform child in transform)
                child.gameObject.SetSortingDetails(
                    sortingLayerId: UnityEngine.SortingLayer.NameToID(SortingLayer),
                    sortingOrder: SortOrder
                );
        }

        public void SetSortingLayer(string sortingLayer)
        {
            SortingLayer = sortingLayer;
            Refresh();
        }

        public void SetSortOrder(int sortingOrder)
        {
            SortOrder = sortingOrder;
            Refresh();
        }
    }
}
